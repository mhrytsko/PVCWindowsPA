using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    /// <summary>
    /// Representa uma cor de janela do sistema.
    /// </summary>
    public class WindowColor : CrudModel
    {
        /// <summary>
        /// Nome da cor da janela.
        /// </summary>
        [Required, MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Código hexadecimal da cor da janela.
        /// </summary>
        [MaxLength(7)]
        public string? HexCode { get; set; }

        [ForeignKey("Brand")]
        public Guid? BrandId { get; set; }

        /// <summary>
        /// Marca
        /// </summary>
        public virtual Brand? Brand { get; set; }

        public Guid? ImageId { get; set; } = null;

        /// <summary>
        /// Imagem do pattern (no caso da laminagem)
        /// </summary>
        [ForeignKey(nameof(ImageId))]
        public Image? Image { get; set; }

        /// <summary>
        /// Tipo da cor
        /// </summary>
        public Framework.BaseEnums.ColorType ColorType { get; set; } = Framework.BaseEnums.ColorType.Solid;
    }
}
