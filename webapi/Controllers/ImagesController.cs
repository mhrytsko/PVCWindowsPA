using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using webapi.Database;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : BaseController
    {

        public ImagesController(IConfiguration config, AppDbContext context, IMapper mapper, ILogger<ImagesController> logger)
            : base(config, context, mapper, logger) { }

        [HttpGet("image/{id}"), Authorize(Policy = "Public")]
        public async Task<IResult> GetImageFile(Guid id)
        {
            return await base.GetImage(_context.Images, id, (i) => i);
        }

        [HttpPost("GetWindow2DImage"), Authorize(Policy = "Public")]
        public async Task<IResult> GetWindow2DImage(Models.Window window, bool withSchema = false, bool withSize = false, bool applyTextures = false)
        {
            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());

            window.WindowProfile = _context.WindowProfiles.FirstOrDefault(wp => wp.Id == window.WindowProfileId);

            if (window.WindowProfile != null)
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


            var janelas = new List<Models.Window>() { window };

            Framework.Generate2DWindow.GenerateImage(window, out MemoryStream stream, withSchema, withSize, applyTextures);

            var fileName = string.Format("janela_{0}.png", Guid.NewGuid().ToString());

            return TypedResults.File(stream, "image/png", fileName);
        }
    }
}
