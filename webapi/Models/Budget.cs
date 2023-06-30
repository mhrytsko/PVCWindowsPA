using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    /// <summary>
    /// Representa um orçamento associado a um utilizador e suas janelas.
    /// </summary>
    [Table("Budgets")]
    public class Budget : CrudModel
    {
        //[Required]
        public int? BudgetNumber { get; set; } = null;

        /// <summary>
        /// O identificador do utilizador associado ao orçamento.
        /// </summary>
        [ForeignKey("User")/*, Required*/]
        public Guid? UserId { get; set; }

        /// <summary>
        /// O identificador do cliente associado ao orçamento.
        /// </summary>
        [ForeignKey("Client")/*, Required*/]
        public Guid? ClientId { get; set; }

        /// <summary>
        /// O utilizador associado ao orçamento.
        /// </summary>
        [JsonIgnore]
        public virtual User? User { get; set; }

        /// <summary>
        /// Cliente - destinatario do orçamento
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction), JsonIgnore]
        public virtual Client? Client { get; set; }

        /// <summary>
        /// Lista de janelas associadas ao orçamento.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual List<BudgetWindow> BudgetWindows { get; set; } = new List<BudgetWindow>();
    }
}
