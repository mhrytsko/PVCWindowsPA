using AutoMapper;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.IO.Compression;
using webapi.Database;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : ControllerBase
    {
        protected readonly AppDbContext _context;

        public ToolsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("DetectWindow"), Authorize(Policy = "Public")]
        public async Task<IResult> DetectWindow(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var imageAnalysis = new webapi.Tools.ImageAnalysisAI();
                    var result = await imageAnalysis.DetectObjects(file);

                    return TypedResults.Ok(result);
                }
            }
            catch
            {
                return TypedResults.Ok(new { success = false });
            }

            return TypedResults.Ok(new { success = false });
        }

        [HttpGet("ModelsSchema"), Authorize(Policy = "Admin")]
        public IResult GetModelsSchema(bool flattenHierarchy = false)
        {
            return TypedResults.Ok(Tools.ModelsSchema.GetModelsSchema(flattenHierarchy));
        }

        [HttpGet("ModelsSchemaAsFile"), Authorize(Policy = "Admin")]
        public async Task<IResult> GetModelsSchemaAsFile(bool flattenHierarchy = false)
        {
            var models = Tools.ModelsSchema.GetModelsSchema(flattenHierarchy);

            // Crie um MemoryStream para manter o arquivo zip em memória até que esteja pronto para ser enviado
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var model in models)
                {
                    var zipEntry = archive.CreateEntry(string.Format("{0}.json", model.Key));
                    using var zipStream = new StreamWriter(zipEntry.Open());
                    await zipStream.WriteAsync(model.Value);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return TypedResults.File(memoryStream, "application/zip", "modelsSchema.zip");
        }

        [HttpGet("ModelsData"), Authorize(Policy = "Admin")]
        public async Task<IResult> GetModelsData()
        {
            var models = new Hashtable();

            // Marcas
            var brands = _context.Brands
                                .Include(b => b.Image)
                                .ToList();

            foreach (var brand in brands)
            {
                brand.SetSerializeAllFields(true);
                brand.Image?.SetSerializeAllFields(true);
            }

            models.Add("brands", brands);

            // Perfils
            var windowProfiles = _context.WindowProfiles
                                .Include(b => b.Image)
                                .AsNoTracking()
                                .ToList();

            foreach (var windowProfile in windowProfiles)
            {
                windowProfile.SetSerializeAllFields(true);
                windowProfile.Image?.SetSerializeAllFields(true);
            }

            models.Add("windowProfiles", windowProfiles);

            // Perfils color
            var windowColors = _context.WindowColors
                                .Include(b => b.Image)
                                .AsNoTracking()
                                .ToList();

            foreach (var windowColor in windowColors)
            {
                windowColor.SetSerializeAllFields(true);
                windowColor.Image?.SetSerializeAllFields(true);
            }

            models.Add("windowColors", windowColors);

            // Crie um MemoryStream para manter o arquivo zip em memória até que esteja pronto para ser enviado
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var modelName in models.Keys)
                {
                    var model = models[modelName];
                    var zipEntry = archive.CreateEntry(string.Format("{0}.json", modelName));
                    using var zipStream = new StreamWriter(zipEntry.Open());
                    var json = System.Text.Json.JsonSerializer.Serialize(model);
                    await zipStream.WriteAsync(json);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return TypedResults.File(memoryStream, "application/zip", "modelsData.zip");
        }

        [HttpPost("RestoreModelsData"), Authorize(Policy = "Admin")]
        public async Task<IResult> RestoreModelsData(IFormFile file)
        {
            var log = new List<object>();
            try
            {
                if (file != null && file.Length > 0)
                {
                    var hasChanges = false;

                    log.Add(new
                    {
                        canConnect = _context.Database.CanConnect(),
                        connectionString = _context.Database.GetConnectionString(),
                    });

                    using (var zipArchive = new ZipArchive(file.OpenReadStream(), ZipArchiveMode.Read))
                    {
                        foreach (var entry in zipArchive.Entries)
                        {
                            if (entry.FullName.EndsWith(".json"))
                            {
                                using var streamReader = new StreamReader(entry.Open());
                                var json = await streamReader.ReadToEndAsync();

                                if (entry.FullName == "brands.json")
                                {
                                    var data = System.Text.Json.JsonSerializer.Deserialize<List<Models.Brand>>(json);
                                    log.Add(new { fileName = "brands", data });
                                    if (data != null)
                                    {
                                        hasChanges = true;
                                        await _context.Brands.AddRangeAsync(data);
                                    }
                                }
                                else if (entry.FullName == "windowProfiles.json")
                                {
                                    var data = System.Text.Json.JsonSerializer.Deserialize<List<Models.WindowProfile>>(json);
                                    log.Add(new { fileName = "windowProfiles", data });
                                    if (data != null)
                                    {
                                        hasChanges = true;
                                        await _context.WindowProfiles.AddRangeAsync(data);
                                    }
                                }
                                else if (entry.FullName == "windowColors.json")
                                {
                                    var data = System.Text.Json.JsonSerializer.Deserialize<List<Models.WindowColor>>(json);
                                    log.Add(new { fileName = "windowColors", data });
                                    if (data != null)
                                    {
                                        hasChanges = true;
                                        await _context.WindowColors.AddRangeAsync(data);
                                    }
                                }
                            }
                        }
                    }

                    log.Add(new { hasChanges });

                    if (hasChanges)
                        await _context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                log.Add(ex.ToString());
            }

            return TypedResults.Ok(log);
        }
    }
}
