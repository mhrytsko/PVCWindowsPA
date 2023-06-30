using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    public interface IModelBase
    {

    }

    public abstract class ModelBase : IModelBase
    {

        /// <summary>
        /// Obter o modelo em formato JSON, para armazenar em backup,
        /// e recuperar no arranque do projeto do zero
        /// </summary>
        /// <returns></returns>
        public virtual string? GetModelAsJson()
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(this);
                return json;
            }
            catch
            {
                // TODO: Log error
                return null;
            }
        }

        [JsonIgnore, NotMapped, Bindable(false)]
        public bool SerializeAllFields { get; protected set; } = false;
        public void SetSerializeAllFields(bool value) { SerializeAllFields = value; }
    }
}
