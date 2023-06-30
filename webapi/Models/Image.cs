using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    public class Image : CrudModel
    {
        

        [BindProperty(Name = "File")]
        public byte[] File { get; set; } = Array.Empty<byte>();
        public bool ShouldSerializeFile() => SerializeAllFields;

        [JsonIgnore, NotMapped, BindProperty(Name = "FileData")]
        public IFormFile? FileData { get; set; } = default!;

        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;

        public void ReadFile()
        {
            if (FileData == null)
                return;

            using var memoryStream = new MemoryStream();
            FileData.CopyToAsync(memoryStream).Wait();

            File = memoryStream.ToArray();
            FileName = FileData.FileName;
            FileType = FileData.ContentType;

            if (Id == Guid.Empty)
                GenerateId();
        }
    }
}
