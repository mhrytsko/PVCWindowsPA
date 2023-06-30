using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using webapi.Framework.BaseEnums;
using System.ComponentModel;
using webapi.Framework;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public interface ICrudModel
    {
        public Guid Id { get; set; }
        public RowState State { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public void GenerateId();
    }

    public abstract class CrudModel : ModelBase, ICrudModel
    {
        /// <summary>
        /// O identificador único
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(NullableGuidConverter))]
        public Guid Id { get; set; } = Guid.Empty;

        public RowState State { get; set; } = RowState.Invalid;

        public DateTime? CreationDate { get; set; } = null;

        public DateTime? ModificationDate { get; set; } = null;

        public void GenerateId()
        {
            Id = Guid.NewGuid();
        }

        public CrudModel() 
        {
            GenerateId();
        }
    }
}
