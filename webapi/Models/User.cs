using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using webapi.Framework.BaseEnums;
using Microsoft.AspNetCore.Identity;
using webapi.Framework;

namespace webapi.Models
{
    /// <summary>
    /// Representa um utilizador do sistema.
    /// </summary>
    [Index(nameof(UserName), IsUnique = true)]
    //[Table("Users")]
    public class User : IdentityUser<Guid>, ICrudModel
    {
        public const string USER_GUEST = "guest";
        public const string USER_ADMIN = "admin";

        public const string USER_GUEST_PASSWORD = "guest";
        public const string USER_ADMIN_PASSWORD = "admin";

        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), PersonalData]
        [JsonConverter(typeof(NullableGuidConverter))]
        public override Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [Required, MaxLength(50), ProtectedPersonalData]
        public override string? UserName { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [JsonIgnore, PasswordPropertyText]
        public override string? PasswordHash { get; set; }

        public PermissionLevel Role { get; set; }


        [ForeignKey("PersonalData")]
        public Guid? PersonalDataId { get; set; }
        public virtual PersonalDetail? PersonalDetail { get; set; }

        public DateTime? CreationDate { get; set; } = null;

        public DateTime? ModificationDate { get; set; } = null;

        public RowState State { get; set; }


        /// <summary>
        /// Coleção de janelas associadas a este utilizador.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.ClientCascade)]
        public virtual List<Window>? Windows { get; set; } = null;

        /// <summary>
        /// Coleção de orçamentos associados a este utilizador.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual List<Budget>? Budgets { get; set; } = null;

        /// <summary>
        /// Obtém ou define a coleção de clientes associadas à janela.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual List<Client>? Clients { get; set; } = null;

        public void GenerateId()
        {
            Id = Guid.NewGuid();
        }

        public User() 
            : base()
        {
            GenerateId();
            PersonalDataId = null;
        }
    }
}
