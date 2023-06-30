using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.Database;
using webapi.Helpers;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WindowGlassTypesController : BaseController
    {
        public WindowGlassTypesController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<WindowGlassTypesController> logger)
            : base(config, context, mapper, logger) { }

        // GET: api/<Controller>
        [HttpGet, Authorize(Policy = "Public")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "Name"
            };
            query.SearchBy = "Name";

            return await GetEntries(_context.WindowGlassTypes, query);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.WindowGlassTypes, id);
        }

        // POST api/<Controller>
        [HttpPost("New"), Authorize(Policy = "Admin")]
        public async Task<IResult> New([FromForm] Models.WindowGlassType value)
        {
            return await CreateEntry(_context.WindowGlassTypes, value, null, (entry) =>
            {
                entry.Image?.GenerateId();
                entry.Image?.ReadFile();
            });
        }

        // PUT api/<Controller>/5
        [HttpPost("Edit"), Authorize(Policy = "Admin")]
        public async Task<IResult> Edit([FromForm] Models.WindowGlassType value)
        {
            return await UpdateEntry(_context.WindowGlassTypes, value, null, (entry, oldValues) => entry.Image?.ReadFile());
        }

        // DELETE api/<Controller>/5
        [HttpPost("Delete"), Authorize(Policy = "Admin")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.WindowGlassTypes, id, null, async (entry) =>
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
