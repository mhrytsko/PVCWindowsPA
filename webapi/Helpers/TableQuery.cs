using System.Linq.Expressions;

namespace webapi.Helpers
{
    public class TableQuery
    {
        public string? SearchBy { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public string? SortBy { get; set; }

        public string? Order { get; set; } = "asc";
        public bool Descending { get { return Order == "desc"; } }

        /// <summary>
        /// Lambda para o getter do campo
        /// </summary>
        /// <typeparam name="DestinationObject"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<DestinationObject, object>>? GetFieldByName<DestinationObject>(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                // Dividir o caminho da propriedade em partes
                string[] propertyParts = propertyName.Split('.', StringSplitOptions.RemoveEmptyEntries);

                // Criar um parâmetro de expressão para a expressão lambda
                ParameterExpression parameter = Expression.Parameter(typeof(DestinationObject), "x");

                // Criar a expressão inicial usando o parâmetro
                Expression currentExpression = parameter;

                // Criar expressões consecutivas para cada parte do caminho da propriedade
                foreach (string propertyPart in propertyParts)
                {
                    // Aceder a próxima propriedade no caminho
                    currentExpression = Expression.PropertyOrField(currentExpression, propertyPart);
                }

                // Converter a expressão final para o tipo 'object'
                //UnaryExpression convertedExpression = Expression.Convert(currentExpression, typeof(object));

                // Criar a expressão de acesso ao campo desejado
                //MemberExpression property = Expression.PropertyOrField(parameter, propertyName);

                // Adicionar uma conversão explícita para o tipo object
                UnaryExpression convertedExpression = Expression.Convert(currentExpression, typeof(object));

                // Criar a expressão lambda usando o campo de acesso e o parâmetro
                return Expression.Lambda<Func<DestinationObject, object>>(convertedExpression, parameter);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lambda com base no campo de pesquisa
        /// </summary>
        /// <typeparam name="DestinationObject"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<DestinationObject, bool>>? GetSearchExpresion<DestinationObject>(string? propertyName, object? value)
        {
            if(string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                ParameterExpression parametro = Expression.Parameter(typeof(DestinationObject), "x");
                MemberExpression propriedade = Expression.PropertyOrField(parametro, propertyName);
                ConstantExpression constante = Expression.Constant(value);
                MethodCallExpression contains = Expression.Call(propriedade, "Contains", Type.EmptyTypes, constante);

                /*
                    MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    MethodCallExpression contains = Expression.Call(propriedade, containsMethod, Expression.Constant(valorPesquisa));
                 */

                return Expression.Lambda<Func<DestinationObject, bool>>(contains, parametro);
            }
            catch
            {
                return null;
            }
        }

    }
}
