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
    public class ClientsController : BaseController
    {
        private readonly UserContext _userContext;

        public ClientsController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<ClientsController> logger, UserContext userContext)
            : base(config, context, mapper, logger)
        {
            _userContext = userContext;
        }

        [HttpGet, Authorize(Policy = "Technic")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "PersonalDetail.FirstName",
                Order = "asc"
            };
            query.SearchBy = "PersonalDetail.FirstName";
            if (string.IsNullOrEmpty(query.SortBy))
            {
                query.SortBy = "PersonalDetail.FirstName";
                query.Order = "asc";
            }

            return await GetEntries(_context.Clients.Include(c => c.PersonalDetail), query, (dbset) => dbset.Where(w => w.ManagerId == _userContext.UserId));
        }

        [HttpGet("{id}"), Authorize(Policy = "Technic")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.Clients, id, (dbset) => dbset.Include(c => c.PersonalDetail).Where(w => w.ManagerId == _userContext.UserId));
        }

        [HttpPost("New"), Authorize(Policy = "Technic")]
        public async Task<IResult> New(Models.Client value)
        {
            return await CreateEntry(_context.Clients, value, null, (entry) =>
            {
                entry.ManagerId = _userContext.UserId!.Value;
                entry.PersonalDetail?.GenerateId();
            });
        }

        [HttpPost("Edit"), Authorize(Policy = "Technic")]
        public async Task<IResult> Edit(Models.Client value)
        {
            return await UpdateEntry(_context.Clients, value, (entry, oldValues) => oldValues.ManagerId == _userContext.UserId, (entry, oldValues) => {
                entry.ManagerId = _userContext.UserId!.Value;

                if (entry.PersonalDetail != null)
                {
                    if (entry.PersonalDetail.Id == Guid.Empty)
                    {
                        entry.PersonalDetail.GenerateId();
                        _context.Entry(entry.PersonalDetail).State = EntityState.Added;
                    }
                    else
                        _context.Entry(entry.PersonalDetail).State = EntityState.Modified;
                }
            });
        }

        [HttpPost("Delete"), Authorize(Policy = "Technic")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.Clients, id, (entry) =>
            {
                return entry.ManagerId == _userContext.UserId;
            },
            async (entry) =>
            {
                // Remover os dados associados
                await _context.Entry(entry)
                    .Reference(b => b.PersonalDetail)
                    .LoadAsync();

                if (entry.PersonalDetail != null)
                    _context.PersonalDetails.Remove(entry.PersonalDetail);
            });
        }
    }
}
