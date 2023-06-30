using NJsonSchema;
using NJsonSchema.Generation;

namespace webapi.Tools
{
    public class ModelsSchema
    {
        public static Dictionary<string, string> GetModelsSchema(bool flattenInheritanceHierarchy = false)
        {
            var result = new Dictionary<string, string>();

            var settings = new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = flattenInheritanceHierarchy,
                
            };

            // ModelBase
            {
                var schema = JsonSchema.FromType<Models.ModelBase>(settings);
                var schemaData = schema.ToJson();
                result.Add("modelBase", schemaData);
            }

            // CrudModel
            {
                var schema = JsonSchema.FromType<Models.CrudModel>(settings);
                var schemaData = schema.ToJson();
                result.Add("crudModel", schemaData);
            }

            // Brand
            {
                var schema = JsonSchema.FromType<Models.Brand>(settings);
                var schemaData = schema.ToJson();
                result.Add("brand", schemaData);
            }

            // Budget
            {
                var schema = JsonSchema.FromType<Models.Budget>(settings);
                var schemaData = schema.ToJson();
                result.Add("budget", schemaData);
            }

            // LeafConfiguration
            {
                var schema = JsonSchema.FromType<Models.LeafConfiguration>(settings);
                var schemaData = schema.ToJson();
                result.Add("leafConfiguration", schemaData);
            }

            // PersonalDetail
            {
                var schema = JsonSchema.FromType<Models.PersonalDetail>(settings);
                var schemaData = schema.ToJson();
                result.Add("personalDetail", schemaData);
            }

            // User
            {
                var schema = JsonSchema.FromType<Models.User>(settings);
                var schemaData = schema.ToJson();
                result.Add("user", schemaData);
            }

            // Client
            {
                var schema = JsonSchema.FromType<Models.Client>(settings);
                var schemaData = schema.ToJson();
                result.Add("client", schemaData);
            }

            // Window
            {
                var schema = JsonSchema.FromType<Models.Window>(settings);
                var schemaData = schema.ToJson();
                result.Add("window", schemaData);
            }

            // WindowColor
            {
                var schema = JsonSchema.FromType<Models.WindowColor>(settings);
                var schemaData = schema.ToJson();
                result.Add("windowColor", schemaData);
            }

            // WindowGlassType
            {
                var schema = JsonSchema.FromType<Models.WindowGlassType>(settings);
                var schemaData = schema.ToJson();
                result.Add("windowGlassType", schemaData);
            }

            // WindowProfile
            {
                var schema = JsonSchema.FromType<Models.WindowProfile>(settings);
                var schemaData = schema.ToJson();
                result.Add("windowProfile", schemaData);
            }

            return result;
        }
    }
}
