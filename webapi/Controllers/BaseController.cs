using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using webapi.Database;
using webapi.Helpers;
using webapi.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace webapi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IConfiguration _config;
        protected readonly AppDbContext _context;
        protected readonly IMapper _mapper;
        private readonly ILogger<BaseController> _logger;

        public BaseController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<BaseController> logger)
        {
            _config = config;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [NonAction]
        protected async Task<IResult> GetEntry<T>(DbSet<T> dbSet, Guid id, Func<DbSet<T>, IQueryable<T>>? beforeSelect = null)
            where T : class, ICrudModel
        {
            IQueryable<T> table = beforeSelect?.Invoke(dbSet) ?? dbSet;

            var entry = await table
               .Where(u => u.Id == id)
               .FirstAsync();

            if (entry == null)
                return TypedResults.NotFound();

            return TypedResults.Ok(entry);
        }

        [NonAction]
        protected async Task<IResult> GetEntries<T>(IQueryable<T> dbSet, TableQuery? query, Func<IQueryable<T>, IQueryable<T>>? afterQueryApply = null)
            where T : class, ICrudModel
        {
            query ??= new TableQuery();

            var qTable = ApplyTableQuery(dbSet, query, out int itemsLength);

            if(afterQueryApply != null)
                qTable = afterQueryApply?.Invoke(qTable);

            if (qTable == null)
                return TypedResults.Problem();

            var items = await qTable.ToListAsync();

            return TypedResults.Ok(new { items, itemsLength });
        }

        private static string[] ModelStateEntryErrors(ModelStateEntry? modelStateEntry)
        {
            return modelStateEntry?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>();
        }

        [NonAction]
        protected Dictionary<string, string[]> GetModelErros()
        {
            return ModelState
                .Where(ms => ms.Value != null && ms.Value.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => ModelStateEntryErrors(kvp.Value)
                );
        }

        [NonAction]
        protected IResult ReturnErrors(IEnumerable<IdentityError> errors)
        {
            return TypedResults.ValidationProblem(errors.ToDictionary(e => e.Code, e => new string[] { e.Description }));
        }

        [NonAction]
        protected IQueryable<T> ApplyTableQuery<T>(IQueryable<T> dbSet, TableQuery? tQuery, out int totalItems) where T : class
        {
            tQuery ??= new TableQuery();
            IQueryable<T> query = dbSet;

            // Aplica a filtragem com base no parâmetro de pesquisa
            if (!string.IsNullOrEmpty(tQuery.Search))
            {
                var searchBy = TableQuery.GetSearchExpresion<T>(tQuery.SearchBy, tQuery.Search);
                if(searchBy != null)
                    query = query.Where(searchBy);
            }

            // Aplica a ordenação
            var sortBy = TableQuery.GetFieldByName<T>(tQuery.SortBy);
            if (sortBy != null)
                query = tQuery.Descending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);

            // Calcula o total de itens
            totalItems = query.Count();

            // Aplica a paginação ( -1 => All )
            if(tQuery.RowsPerPage > 0)
                query = query.Skip((tQuery.Page - 1) * tQuery.RowsPerPage).Take(tQuery.RowsPerPage);

            return query;
        }

        public class CustomValidation
        {
            public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
            public string? Detail { get; set; } = null;
            public string? Instance { get; set; } = null;
            public string? Title { get; set; } = null;
            public string? Type { get; set; } = null;
            public IDictionary<string, object?>? Extensions { get; set; } = null;

            public bool IsValid { 
                get
                {
                    return (Errors?.Any() ?? false) == false;
                }
            }
        }

        [NonAction]
        protected async Task<IResult> DeleteEntry<T>(DbSet<T> dbSet, Guid id, Func<T, bool>? validation = null, Func<T, Task>? beforeRemove = null) where T : class
        {
            var row = await dbSet.FindAsync(id);
            if (row == null)
                return TypedResults.NotFound();

            if (validation != null && !validation(row))
                return TypedResults.ValidationProblem(GetModelErros());

            if (beforeRemove != null)
                await beforeRemove(row);

            dbSet.Remove(row);
            await _context.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        [NonAction]
        protected bool EntryExists<T>(IQueryable<T> dbSet, Guid id)
            where T : class, ICrudModel
        {
            return dbSet.Any(r => r.Id == id);
        }

        [NonAction]
        protected async Task<IResult> CreateEntry<T>(DbSet<T> dbSet, T entry, Func<T, bool>? validation = null, Action<T>? beforeCreate = null)
            where T : class, ICrudModel
        {
            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());
            try
            {
                if (validation != null && !validation(entry))
                    return TypedResults.ValidationProblem(GetModelErros());

                entry.GenerateId();
                entry.State = Framework.BaseEnums.RowState.Valid;

                beforeCreate?.Invoke(entry);

                await dbSet.AddAsync(entry);
                await _context.SaveChangesAsync();

                return TypedResults.Ok(new { id = entry.Id });
            }
            catch (DbUpdateException updateException)
            {
                return TypedResults.Problem(updateException.Message);
            }
            catch
            {
                return TypedResults.Problem("Ups, algum problema... :(");
            }
        }

        [NonAction]
        protected async Task<IResult> UpdateEntry<T>(DbSet<T> dbSet, T entry, Func<T, T, bool>? validation = null, Action<T, T>? beforeUpdate = null)
            where T : class, ICrudModel
        {
            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());
            try
            {
                var oldValues = await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entry.Id);

                if (oldValues == null)
                    return TypedResults.NotFound();

                if (validation != null && !validation(entry, oldValues))
                    return TypedResults.ValidationProblem(GetModelErros());

                entry.State = Framework.BaseEnums.RowState.Valid;

                beforeUpdate?.Invoke(entry, oldValues);

                dbSet.Update(entry);
                await _context.SaveChangesAsync();

                return TypedResults.Ok(new { id = entry.Id });
            }
            catch (DbUpdateException updateException)
            {
                return TypedResults.Problem(updateException.Message);
            }
            catch
            {
                return TypedResults.Problem("Ups, algum problema... :(");
            }
        }

        [NonAction]
        protected async Task<IResult> GetImage<T>(DbSet<T> dbSet, Guid id, Func<T, Models.Image?> fnImageField, string? include = null)
            where T : class, ICrudModel
        {
            T? entry;
            if (string.IsNullOrEmpty(include))
                entry = await dbSet.FindAsync(id);
            else
                entry = await dbSet.Include(include).FirstOrDefaultAsync(e => e.Id == id);
            
            if(entry == null)
                return TypedResults.NotFound();

            var image = fnImageField(entry);

            if(!(image?.File?.Any() ?? false))
                return TypedResults.Empty;

            var fileExtension = Path.GetExtension(image?.FileName) ?? string.Empty;
            var extensionProvider = new FileExtensionContentTypeProvider();

            string contentType = extensionProvider.Mappings.TryGetValue(fileExtension, out var type) ? type : "application/octet-stream";

            Microsoft.Net.Http.Headers.EntityTagHeaderValue? entityTag = null;
            /*var imageDT = image?.ModificationDate ?? image?.CreationDate;
            if(imageDT != null && image?.Id != null)
            {
                var timestamp = imageDT.Value.Ticks.ToString();
                var eTag = string.Format("If-None-Match: \"{0}-{1}\"", image?.Id, timestamp);
                var a = Microsoft.Net.Http.Headers.EntityTagHeaderValue.Parse(eTag);
                entityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue(eTag);
            }*/

            return TypedResults.File(image?.File ?? Array.Empty<byte>(), contentType, lastModified: image?.ModificationDate, entityTag: entityTag);
        }
    }
}
