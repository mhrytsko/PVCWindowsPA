using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using webapi.Framework;
using webapi.Framework.BaseEnums;

namespace webapi.Models
{
    /// <summary>
    /// Representa a configuração de uma folha em uma janela.
    /// </summary>
    public class LeafConfiguration : CrudModel
    {
        /// <summary>
        /// Identificador da janela associada à configuração da folha.
        /// </summary>
        [ForeignKey("Window"), Required(ErrorMessage = "A folha da janela, obrigatóriamente tem estar associada a janela. Por favor, preencha o campo.")]
        public Guid? WindowId { get; set; }

        /// <summary>
        /// Identificador do tipo de vidro da janela associado à configuração da folha.
        /// </summary>
        [ForeignKey("WindowGlassType")]
        public Guid? WindowGlassTypeId { get; set; }

        /// <summary>
        /// Tipo de abertura
        /// </summary>
        [Required(ErrorMessage = "O tipo de sistema de abertura da folha é obrigatório. Por favor, preencha o campo.")]
        public WindowOpeningType OpeningSystem { get; set; } = WindowOpeningType.Fixed;

        /// <summary>
        /// Tipo de fecho da janela
        /// </summary>
        [Required(ErrorMessage = "A direção da abertura da folha é obrigatório. Por favor, preencha o campo.")]
        public WindowOpeningDirection OpeningDirection { get; set; } = WindowOpeningDirection.None;

        /// <summary>
        /// Se tem puxador / fechadura ?
        /// </summary>
        public bool HasHandle { get; set; } = false;

        /// <summary>
        /// Largura da folha na configuração.
        /// </summary>
        [Required(ErrorMessage = "A largura da folha é obrigatória. Por favor, preencha o campo.")]
        public int Width { get; set; } = 0;

        /// <summary>
        /// Altura da folha na configuração.
        /// </summary>
        [Required(ErrorMessage = "A altura da folha é obrigatória. Por favor, preencha o campo.")]
        public int Height { get; set; } = 0;

        /// <summary>
        /// Linha da posição (coordenada X)
        /// </summary>
        [Required(ErrorMessage = "A posição X da folha é obrigatória. Por favor, preencha o campo.")]
        public int? X { get; set; } = null;

        /// <summary>
        /// Coluna da posição (coordenada Y)
        /// </summary>
        [Required(ErrorMessage = "A posição Y da folha é obrigatória. Por favor, preencha o campo.")]
        public int? Y { get; set; } = null;

        /// <summary>
        /// Obtém ou define o tipo de vidro da janela associado à configuração da folha.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowGlassType? WindowGlassType { get; set; }

        /// <summary>
        /// Fosco ?
        /// </summary>
        public bool Frosted { get; set; } = false;


        /// <summary>
        /// Janela associada à configuração da folha.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction), JsonIgnore]
        public virtual Window? Window { get; set; }
    }
}
