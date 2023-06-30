using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using webapi.Framework.BaseEnums;
using webapi.Models;
using webapi.Framework;

namespace webapi.ViewModels
{
    public class UserData
    {
        [Key]
        [JsonConverter(typeof(NullableGuidConverter))]
        public Guid Id { get; set; } = default!;

        [Required, MaxLength(50)]
        public string? UserName { get; set; }

        /// <summary>
        /// Só para criação / atualização pelo interface
        /// </summary>
        [PasswordPropertyText]
        public string? Password { get; set; }

        public PermissionLevel Role { get; set; }

        public PersonalDetail? PersonalDetail { get; set; }

        public RowState State { get; set; }

        public string? SecurityStamp { get; set; }
    }
}
