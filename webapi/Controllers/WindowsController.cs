using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Framework;
using webapi.Helpers;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WindowsController : BaseController
    {
        private readonly UserContext _userContext;

        public WindowsController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<WindowsController> logger, UserContext userContext)
            : base(config, context, mapper, logger)
        {
            _userContext = userContext;
        }

        [HttpGet, Authorize(Policy = "Client")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "CreationDate",
                Order = "desc"
            };
            query.SearchBy = "";
            if (string.IsNullOrEmpty(query.SortBy))
            {
                query.SortBy = "CreationDate";
                query.Order = "desc";
            }

            return await GetEntries(_context.Windows.Include(w => w.LeafConfigurations), query, (dbset) => dbset.Where(w => w.UserId == _userContext.UserId));
        }

        [HttpGet("{id}"), Authorize(Policy = "Client")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.Windows, id, (dbset) => dbset.Include(w => w.LeafConfigurations).Where(w => w.UserId == _userContext.UserId));
        }

        [HttpPost("New"), Authorize(Policy = "Client")]
        public async Task<IResult> New(Models.Window value)
        {
            return await CreateEntry(_context.Windows, value, null, (entry) =>
            {
                entry.UserId = _userContext.UserId;
                entry.LeafConfigurations?.ForEach(lc => lc.GenerateId());
            });
        }

        [HttpPost("Edit"), Authorize(Policy = "Client")]
        public async Task<IResult> Edit(Models.Window value)
        {
            return await UpdateEntry(_context.Windows, value, (entry, oldValues) => oldValues.UserId == _userContext.UserId, (entry, oldValues) => {
                entry.UserId = _userContext.UserId;

                // Remover as associações
                var oldRows = _context.LeafConfigurations
                    .AsNoTracking()
                    .Where(lf => lf.WindowId == oldValues.Id)
                    .ToList();
                var removedLeaf = oldRows.Except(entry.LeafConfigurations!);
                _context.LeafConfigurations.RemoveRange(removedLeaf);

                // Adicionar as novas folhas
                var newLeaf = entry.LeafConfigurations!.Except(oldRows).ToList();
                // Gerar os ID's para os novos
                newLeaf.ForEach(lc =>
                {
                    if(lc.Id == Guid.Empty)
                        lc.GenerateId();
                });

                _context.LeafConfigurations.AddRange(newLeaf);

                // Atualizar o estado das folhas da janela
                _context.Entry(entry).Collection(b => b.LeafConfigurations).IsModified = true;
            });
        }

        [HttpPost("Delete"), Authorize(Policy = "Client")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.Windows, id, (entry) =>
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
