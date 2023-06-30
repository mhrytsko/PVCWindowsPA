using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using webapi.Database;
using webapi.Framework;
using webapi.Helpers;
using webapi.Models;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : BaseController
    {
        private readonly UserContext _userContext;

        public BudgetsController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<BudgetsController> logger, UserContext userContext)
            : base(config, context, mapper, logger) 
        {
            _userContext = userContext;
        }

        [HttpPost("GetWindowBudget"), Authorize(Policy = "Public")]
        public async Task<IResult> GetWindowBudget(Models.Window window)
        {
            if(!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());

            window.WindowProfile = _context.WindowProfiles.FirstOrDefault(wp => wp.Id == window.WindowProfileId);

            if(window.WindowProfile != null)
                await _context.Entry(window.WindowProfile)
                    .Reference(wp => wp.Brand)
                    .LoadAsync();

            if (window.IndorColorId != null)
            {
                window.IndorColor = _context.WindowColors.FirstOrDefault(wColor => wColor.Id == window.IndorColorId);

                if (window.IndorColor?.ColorType == Framework.BaseEnums.ColorType.Pattern)
                    await _context.Entry(window.IndorColor)
                        .Reference(wcolor => wcolor.Image)
                        .LoadAsync();
            }

            if (window.OutdorColorId != null)
            {
                window.OutdorColor = _context.WindowColors.FirstOrDefault(wColor => wColor.Id == window.OutdorColorId);

                if (window.OutdorColor?.ColorType == Framework.BaseEnums.ColorType.Pattern)
                    await _context.Entry(window.OutdorColor)
                        .Reference(wcolor => wcolor.Image)
                        .LoadAsync();
            }

            var janelas = new List<Models.Window>() { window };

            Framework.GenerateBudgetPdf.GeneratePdf(janelas, out MemoryStream stream);

            var fileName = string.Format("orçamento_{0}.pdf", Guid.NewGuid().ToString());

            return TypedResults.File(stream, "application/pdf", fileName);
        }

        [HttpPost("GetBudgetById"), Authorize(Policy = "Client")]
        public async Task<IResult> GetBudgetById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                ModelState.AddModelError("id", "O Id do orçamento é inválido!");

            var userId = _userContext.UserId;
            if (userId == null || userId == Guid.Empty)
                ModelState.AddModelError("user", "O utilizador é inválido!");

            var budget = await _context.Budgets
                .Include(b => b.BudgetWindows)
                    .ThenInclude(bw => bw.Window)
                        .ThenInclude(w => w!.LeafConfigurations)
                 .Include(b => b.BudgetWindows)
                    .ThenInclude(bw => bw.Window)
                        .ThenInclude(w => w!.WindowProfile)
                            .ThenInclude(wProfile => wProfile!.Image)
                .Include(b => b.BudgetWindows)
                    .ThenInclude(bw => bw.Window)
                        .ThenInclude(w => w!.WindowProfile)
                            .ThenInclude(p => p!.Brand)
                .Include(b => b.BudgetWindows)
                    .ThenInclude(bw => bw.Window)
                        .ThenInclude(w => w!.IndorColor)
                .Include(b => b.BudgetWindows)
                    .ThenInclude(bw => bw.Window)
                        .ThenInclude(w => w!.OutdorColor)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId && b.State == Framework.BaseEnums.RowState.Valid);

            if (budget == null)
                ModelState.AddModelError("budget", "O orçamento é inválido!");

            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());

            //_context.Entry(budget).Collection(bw => bw.Window!.LeafConfigurations).Load();

            var janelas = budget?.BudgetWindows?.Where(bw => bw.Window != null).Select(bw => bw.Window!).ToList() ?? new List<Window>();

            Framework.GenerateBudgetPdf.GeneratePdf(janelas, out MemoryStream stream, budget?.BudgetNumber, budget?.Client?.PersonalDetail);

            var fileName = string.Format("orçamento_{0}.pdf", Guid.NewGuid().ToString());

            return TypedResults.File(stream, "application/pdf", fileName);
        }

        [HttpGet, Authorize(Policy = "Client")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null, [FromQuery] Guid? clientId = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "BudgetNumber",
                Order = "desc"
            };
            query.SearchBy = "BudgetNumber";
            if (string.IsNullOrEmpty(query.SortBy))
            {
                query.SortBy = "BudgetNumber";
                query.Order = "desc";
            }

            if(clientId != null)
                return await GetEntries(_context.Budgets, query, (dbset) => dbset.Where(b => b.UserId == _userContext.UserId && b.ClientId == clientId));
            return await GetEntries(_context.Budgets, query, (dbset) => dbset.Where(b => b.UserId == _userContext.UserId));
        }

        [HttpGet("{id}"), Authorize(Policy = "Client")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.Budgets, id, (dbset) => dbset.Include(b => b.BudgetWindows).Where(b => b.UserId == _userContext.UserId));
        }

        [HttpPost("New"), Authorize(Policy = "Client")]
        public async Task<IResult> New(Models.Budget value)
        {
            return await CreateEntry(_context.Budgets, value, null, (entry) =>
            {
                entry.GenerateId();
                entry.UserId = _userContext.UserId;
                entry.BudgetWindows?.ForEach(bw => bw.BudgetId = entry.Id);

                var budgetNumber = _context.Budgets
                    .Where(b => b.UserId == _userContext.UserId && b.State == Framework.BaseEnums.RowState.Valid)
                    .Max(b => b.BudgetNumber);
                entry.BudgetNumber = (budgetNumber ?? 0) + 1;

            });
        }

        [HttpPost("Edit"), Authorize(Policy = "Client")]
        public async Task<IResult> Edit(Models.Budget value)
        {
            return await UpdateEntry(_context.Budgets, value, (entry, oldValues) => oldValues.UserId == _userContext.UserId, (entry, oldValues) => {
                entry.UserId = _userContext.UserId;

                entry.BudgetWindows?.ForEach(bw => bw.BudgetId = entry.Id);


                // Remover as associações N:N
                var oldRows = _context.BudgetWindows
                    .AsNoTracking()
                    .Where(bw => bw.BudgetId == oldValues.Id)
                    .ToList();

                var removedWindows = oldRows.Except(entry.BudgetWindows!);
                _context.BudgetWindows.RemoveRange(removedWindows);

                // Adicionar os novos BudgetWindows
                var newWindows = entry.BudgetWindows!.Except(oldRows);
                _context.BudgetWindows.AddRange(newWindows);

                /*if (entry.BudgetWindows == null || !(entry.BudgetWindows?.Any() ?? false))
                    _context.Entry(entry).Collection(b => b.BudgetWindows).CurrentValue = null;
                else
                    _context.Entry(entry).Collection(b => b.BudgetWindows).IsModified = true;*/

                // Atualizar o estado da entidade Budget
                _context.Entry(entry).Collection(b => b.BudgetWindows).IsModified = true;
            });
        }

        [HttpPost("Delete"), Authorize(Policy = "Client")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.Budgets, id, (entry) =>
            {
                return entry.UserId == _userContext.UserId;
            }/*, async (entry) =>
            {
                // Remover as associações N:N
                await _context.Entry(entry)
                    .Reference(b => b.BudgetWindows)
                    .LoadAsync();

                if (entry.BudgetWindows != null)
                    entry.BudgetWindows.ForEach(bw => _context.BudgetWindows.Remove(bw));
            }*/);
        }
    }
}
