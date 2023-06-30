using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Helpers;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WindowProfilesController : BaseController
    {
        public WindowProfilesController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<WindowProfilesController> logger)
            : base(config, context, mapper, logger) { }

        // GET: api/<Controller>
        [HttpGet, Authorize(Policy = "Public")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "CreationDate"
            };
            query.SearchBy = "Name";
            if(string.IsNullOrEmpty(query.SortBy))
            {
                query.SortBy = "CreationDate";
                query.Order = "desc";
            }

            return await GetEntries(_context.WindowProfiles, query, (qTable) => qTable.Include(wp => wp.Brand));
        }

        // GET api/<Controller>/5
        [HttpGet("{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.WindowProfiles, id);
        }

        // POST api/<Controller>
        [HttpPost("New"), Authorize(Policy = "Admin")]
        public async Task<IResult> New([FromForm] Models.WindowProfile value)
        {
            return await CreateEntry(_context.WindowProfiles, value, null, (entry) =>
            {
                entry.Image?.GenerateId();
                entry.Image?.ReadFile();
            });
        }

        // PUT api/<Controller>/5
        [HttpPost("Edit"), Authorize(Policy = "Admin")]
        public async Task<IResult> Edit([FromForm] Models.WindowProfile value)
        {
            return await UpdateEntry(_context.WindowProfiles, value, null, (entry, oldValues) =>
            {
                entry.Image?.ReadFile();
                if (entry.Image != null)
                {
                    if (oldValues?.ImageId == null)
                        _context.Entry(entry.Image).State = EntityState.Added;
                    else
                        _context.Entry(entry.Image).State = EntityState.Modified;
                }
            });
        }

        // DELETE api/<Controller>/5
        [HttpPost("Delete"), Authorize(Policy = "Admin")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.WindowProfiles, id, null, async (entry) =>
            {
                // Remover imagem associada
                await _context.Entry(entry)
                    .Reference(b => b.Image)
                    .LoadAsync();

                if (entry.Image != null)
                    _context.Images.Remove(entry.Image);
            });
        }
    }
}
