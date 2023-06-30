using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    /// <summary>
    /// Representa um tipo de vidro de janela.
    /// </summary>
    public class WindowGlassType : CrudModel
    {
        /// <summary>
        /// Obtém ou define o nome do tipo de vidro da janela.
        /// </summary>
        [Required, MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do tipo de vidro da janela.
        /// </summary>
        [MaxLength(10000)]
        public string? Description { get; set; }

        [ForeignKey("Brand")]
        public Guid? BrandId { get; set; }

        /// <summary>
        /// Marca
        /// </summary>
        public virtual Brand? Brand { get; set; }
        
        public Guid? ImageId { get; set; }

        /// <summary>
        /// Imagem do vidro
        /// </summary>
        [ForeignKey(nameof(ImageId))]
        public Image? Image { get; set; }

        /// <summary>
        /// Espessura do vidro em milímetros (mm).
        /// </summary>
        public double Thickness { get; set; }

        /// <summary>
        /// Número de camadas da janela.
        /// </summary>
        public int ChamberCount { get; set; } = 0;

        /// <summary>
        /// Isolamento térmico - Uf (W/m²K)
        /// </summary>
        public double ThermalInsulation { get; set; } = 0;

        /// <summary>
        /// Isolamento acústico - Rw (dB)
        /// </summary>
        public int SoundInsulation { get; set; } = 0;

        /// <summary>
        /// Resistência antirroubo
        /// </summary>
        public string? AntiTheftResistance { get; set; }

        /// <summary>
        /// Fosco ?
        /// </summary>
        public bool Frosted { get; set; } = false;

        public WindowGlassType()
            :base()
        {
        }
    }
}
