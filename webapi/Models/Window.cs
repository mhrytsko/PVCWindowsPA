using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    /// <summary>
    /// Representa uma janela do sistema.
    /// </summary>
    public class Window : CrudModel
    {
        /// <summary>
        /// Descrição da janela.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Identificador da cor da janela do lado de dentro.
        /// </summary>
        [ForeignKey("IndorColor")/*, Required*/]
        public Guid? IndorColorId { get; set; }

        /// <summary>
        /// Identificador da cor da janela do lado de fora.
        /// </summary>
        [ForeignKey("OutdorColor")/*, Required*/]
        public Guid? OutdorColorId { get; set; }

        /// <summary>
        /// Identificador do perfil da janela.
        /// </summary>
        [ForeignKey("WindowProfile")/*, Required(ErrorMessage = "O Perfil da janela é obrigatório. Por favor, preencha o campo.")*/]
        public Guid? WindowProfileId { get; set; }

        /// <summary>
        /// Identificador do tipo de vidro da janela associado à configuração da folha.
        /// </summary>
        [ForeignKey("WindowGlassType")/*, Required*/]
        public Guid? WindowGlassTypeId { get; set; }

        /// <summary>
        /// Largura da janela
        /// </summary>
        public int? Width { get; set; } = 0;
        /// <summary>
        /// ALtura da janela
        /// </summary>
        public int? Height { get; set; } = 0;

        ///// <summary>
        ///// Obtém ou define o identificador do utilizador associado à janela.
        ///// </summary>
        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// A cor da janela do lado de dentro.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowColor? IndorColor { get; set; }

        /// <summary>
        /// A cor da janela do lado de fora.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowColor? OutdorColor { get; set; }

        /// <summary>
        /// Perfil da janela.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowProfile? WindowProfile { get; set; }

        /// <summary>
        /// Obtém ou define o utilizador associado à janela.
        /// </summary>
        public User? User { get; set; }


        /// <summary>
        /// Tipo de vidro da janela.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual WindowGlassType? WindowGlassType { get; set; }

        /// <summary>
        /// Obtém ou define a coleção de configurações de folhas associadas à janela.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual List<LeafConfiguration> LeafConfigurations { get; set; } = new List<LeafConfiguration>();

        [NotMapped, JsonIgnore]
        public virtual List<List<LeafConfiguration>> LeafConfigurationsMap {
            get
            {
                var leafConfigurations = LeafConfigurations
                    .Where(lc =>lc.State == Framework.BaseEnums.RowState.Valid);

                int maxX = leafConfigurations.Max(lc => lc.X) ?? -1;

                var leafArray = new List<List<LeafConfiguration>>(maxX + 1);

                var groups = leafConfigurations.OrderBy(leaf => leaf.X).ThenBy(leaf => leaf.Y).GroupBy(leaf => leaf.X);
                foreach(var group in groups)
                {
                    leafArray.Add(group.ToList());
                }

                return leafArray;
            }
        }

        public Window() : base()
        {
        }

        public bool ValidarFolhas()
        {
            if (LeafConfigurations?.Any() ?? false)
            {
                // Verificar se todos tenham a Row e Column definidos
                if (LeafConfigurations.Any(lc => lc.X == null || lc.Y == null))
                    return false;

                // Verificar se soma da largura corespode a largura da janela
                if (LeafConfigurations.GroupBy(lc => lc.X).Any(glc => glc.Select(lc => lc.Width).Sum() != this.Width))
                    return false;

                // Verificar se soma da altura corespode a altura da janela
                if (LeafConfigurations.GroupBy(lc => lc.Y).Any(glc => glc.Select(lc => lc.Height).Sum() != this.Height))
                    return false;
            }
            // Não pode ter as dimensões se não tem as folhas definidas
            else if (this.Width != 0 || this.Height != 0)
                return false;

            return true;
        }
    }
}
