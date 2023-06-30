using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    [Table("Clients")]
    public class Client : CrudModel
    {
        /// <summary>
        /// O identificador do gestor / re-vendedor / fábrica
        /// </summary>
        [ForeignKey("Manager")/*, Required*/]
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Identificador para os dados pessoais do cliente
        /// </summary>
        [ForeignKey("PersonalDetail")/*, Required*/]
        public Guid? PersonalDataId { get; set; }

        [NotMapped]
        public string Name => $"{PersonalDetail?.FirstName} {PersonalDetail?.LastName}";

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual User? Manager { get; set; } = null;
        public virtual PersonalDetail? PersonalDetail { get; set; }
    }
}
