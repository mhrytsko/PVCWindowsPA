using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    /// <summary>
    /// Marca
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Brand : CrudModel
    {
        [Required(ErrorMessage = "O nome da marca é obrigatório. Por favor, preencha o campo."), MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(10000)]
        public string? Description { get; set; }

        [Url, DefaultValue(null)]
        public string? Site { get; set; } = null;

        [ForeignKey(nameof(Image))]
        public Guid? ImageId { get; set; }

        [DefaultValue(null)]
        public virtual Image? Image { get; set; } = null;
    }
}
