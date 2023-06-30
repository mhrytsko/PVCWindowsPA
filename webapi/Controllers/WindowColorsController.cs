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
    public class WindowColorsController : BaseController
    {
        public WindowColorsController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<WindowColorsController> logger)
            : base(config, context, mapper, logger) { }

        // GET: api/<WindowColorsController>
        [HttpGet, Authorize(Policy = "Public")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "Name"
            };
            query.SearchBy = "Name";
            /*if (string.IsNullOrEmpty(query.SortBy))
            {
                query.SortBy = "CreationDate";
                query.Order = "desc";
            }*/

            return await GetEntries(_context.WindowColors
                .OrderBy(wcolor => wcolor.ColorType)
                .ThenByDescending(wcolor => wcolor.CreationDate), query);
        }

        // GET api/<WindowColorsController>/5
        [HttpGet("{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.WindowColors, id);
        }

        // POST
        [HttpPost("New"), Authorize(Policy = "Admin")]
        public async Task<IResult> New([FromForm] Models.WindowColor value)
        {
            return await CreateEntry(_context.WindowColors, value, null, (entry) =>
            {
                entry.Image?.GenerateId();
                entry.Image?.ReadFile();
            });
        }

        // PUT
        [HttpPost("Edit"), Authorize(Policy = "Admin")]
        public async Task<IResult> Edit([FromForm] Models.WindowColor value)
        {
            return await UpdateEntry(_context.WindowColors, value, null, (entry, oldValues) => {
                entry.Image?.ReadFile();
                if(entry.Image != null)
                {
                    if(oldValues?.ImageId == null)
                        _context.Entry(entry.Image).State = EntityState.Added;
                    else
                        _context.Entry(entry.Image).State = EntityState.Modified;
                }
            });
        }

        // DELETE
        [HttpPost("Delete"), Authorize(Policy = "Admin")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.WindowColors, id, null, async (entry) =>
            {
                // Remover imagem associada
                await _context.Entry(entry)
                    .Reference(b => b.Image)
                    .LoadAsync();

                if (entry.Image != null)
                    _context.Images.Remove(entry.Image);
            });
        }

        [HttpGet("image/{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> GetImage(Guid id)
        {
            return await base.GetImage(_context.WindowColors, id, (b) => b.Image, "Image");
        }

        [HttpGet("GetTexture/{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> GetTexture(Guid id)
        {
            return await base.GetImage(_context.WindowColors, id, (b) => b.Image, "Image");
        }

        
    }
}
