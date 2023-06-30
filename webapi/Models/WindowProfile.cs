using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    /// <summary>
    /// Representa um perfil de janela.
    /// </summary>
    public class WindowProfile : CrudModel
    {
        /// <summary>
        /// O nome do perfil da janela.
        /// </summary>
        [Required, MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Descrição do perfil da janela.
        /// </summary>
        [MaxLength(10000)]
        public string? Description { get; set; }

        [ForeignKey("Brand"), Required(ErrorMessage = "A marca do perfil da janela é obrigatório. Por favor, preencha o campo.")]
        public Guid? BrandId { get; set; }

        /// <summary>
        /// Marca
        /// </summary>
        public virtual Brand? Brand { get; set; }

        public Guid? ImageId { get; set; }

        /// <summary>
        /// Imagem do perfil
        /// </summary>
        [ForeignKey(nameof(ImageId))]
        public Image? Image { get; set; }

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
        /// Profundidade construtiva (mm)
        /// </summary>
        public int ConstructionDepth { get; set; } = 0;

        /// <summary>
        /// Número de câmaras
        /// </summary>
        public int FrameChamberCount { get; set; } = 0;

        /// <summary>
        /// Permeabilidade ao ar
        /// </summary>
        public string? AirPermeability { get; set; }

        /// <summary>
        /// Estanqueidade à água
        /// </summary>
        public string? WaterTightness { get; set; }

        /// <summary>
        /// Resistência ao vento
        /// </summary>
        public string? WindResistance { get; set; }

        /// <summary>
        /// Espessura máxima do vidro suportada (mm)
        /// </summary>
        public int MaxGlassThickness { get; set; } = 0;

        /// <summary>
        /// Suporta abertura Batente
        /// </summary>
        public bool SideHungOpening { get; set; }
        /// <summary>
        /// Suporta abertura Oscilobatente
        /// </summary>
        public bool TiltAndTurnOpening { get; set; }
        /// <summary>
        /// Suporta abertura Basculante
        /// </summary>
        public bool TiltOnlyOpening { get; set; }
        /// <summary>
        /// Suporta abertura Oscilo-paralelo
        /// </summary>
        public bool TiltAndParallelOpening { get; set; }

        /// <summary>
        /// Dimensões máx. folha - Largura (mm)
        /// </summary>
        public int MaxLeafSizeWidth { get; set; } = 0;
        /// <summary>
        /// Dimensões máx. folha - Altura (mm)
        /// </summary>
        public int MaxLeafSizeHeight { get; set; } = 0;

        /// <summary>
        /// Lista de cores compatíveis.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual IList<WindowProfileColor>? Colors { get; set; }

        public WindowProfile() : base()
        {
        }
    }
}
