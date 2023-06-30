using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Framework.BaseEnums;

namespace webapi.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class PersonalDetail : CrudModel
    {
        /// <summary>
        /// Obtém ou define o primeiro nome do utilizador.
        /// </summary>
        [MaxLength(255)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Obtém ou define o sobrenome do utilizador.
        /// </summary>
        [MaxLength(255)]
        public string? LastName { get; set; }

        /// <summary>
        /// Obtém ou define o e-mail do utilizador.
        /// </summary>
        [MaxLength(255), EmailAddress]
        public string? Email { get; set; }

        [Phone] 
        public string? Phone { get; set; }

        public PersonalDetail() : base()
        {
        }
    }
}
