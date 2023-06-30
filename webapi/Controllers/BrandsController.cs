using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Helpers;
using webapi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BaseController
    {
        public BrandsController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<BrandsController> logger) 
            : base(config, context, mapper, logger) { }

        // GET: api/<BrandsController>
        [HttpGet, Authorize(Policy = "Public")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "Name"
            };
            query.SearchBy = "Name";

            return await GetEntries(_context.Brands, query);
        }

        // GET api/<BrandsController>/5
        [HttpGet("{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> Get(Guid id)
        {
            return await GetEntry(_context.Brands, id);
        }

        // POST
        [HttpPost("New"), Authorize(Policy = "Admin")]
        public async Task<IResult> New([FromForm] Models.Brand value)
        {
            return await CreateEntry(_context.Brands, value, null, (entry) =>
            {
                entry.Image?.GenerateId();
                entry.Image?.ReadFile();
            });
        }

        // PUT
        [HttpPost("Edit"), Authorize(Policy = "Admin")]
        public async Task<IResult> Edit([FromForm] Models.Brand value)
        {
            return await UpdateEntry(_context.Brands, value, null, (entry, oldValues) =>
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

        // DELETE
        [HttpPost("Delete"), Authorize(Policy = "Admin")]
        public async Task<IResult> Delete(Guid id)
        {
            return await DeleteEntry(_context.Brands, id, null, async (entry) =>
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
            return await base.GetImage(_context.Brands, id, (b) => b.Image, "Image");
        }
    }
}
