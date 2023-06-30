using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
//namespace webapi.Framework
{
    /// <summary>
    /// This class overrides the current GUID model binder and makes it a bit smarter in situations where the value is empty or invalid.
    /// The default returned value is to return an Empty Guid which for our purposes should mean the value passed was empty.
    /// </summary>
    /// <remarks>
    /// The GUID binder will cause an invalid model error if no GUID value is specified in the Request data. This is by default a situation that
    /// arises a lot in regard to an optional Guid identity value not being specified in Request.
    /// </remarks>
    public class GuidModelBinder : IModelBinder
    {
        private readonly TypeConverter _typeConverter;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GuidModelBinder"/>.
        /// </summary>
        /// <param name="type">The type to create binder for.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public GuidModelBinder(Type type, ILoggerFactory loggerFactory)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _typeConverter = TypeDescriptor.GetConverter(type);
            _logger = loggerFactory.CreateLogger<GuidModelBinder>();
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            //_logger.AttemptingToBindModel(bindingContext);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                //_logger.FoundNoValueInRequest(bindingContext);

                // no entry
                //_logger.DoneAttemptingToBindModel(bindingContext);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;

                object? model;

                if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out Guid result))
                {
                    if (bindingContext.ModelType == typeof(Guid?))
                        model = null;
                    else
                        model = Guid.Empty;
                }
                else
                    model = result;

                /*else
                {
                    model = _typeConverter.ConvertFrom(
                        context: null,
                        culture: valueProviderResult.Culture,
                        value: value);
                }*/


                CheckModel(bindingContext, valueProviderResult, model);

                //_logger.DoneAttemptingToBindModel(bindingContext);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                var isFormatException = exception is FormatException;
                if (!isFormatException && exception.InnerException != null)
                {
                    // TypeConverter throws System.Exception wrapping the FormatException,
                    // so we capture the inner exception.
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
                }

                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    exception,
                    bindingContext.ModelMetadata);

                // Were able to find a converter for the type but conversion failed.
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// If the <paramref name="model" /> is <see langword="null" />, verifies that it is allowed to be <see langword="null" />,
        /// otherwise notifies the <see cref="P:ModelBindingContext.ModelState" /> about the invalid <paramref name="valueProviderResult" />.
        /// Sets the <see href="P:ModelBindingContext.Result" /> to the <paramref name="model" /> if successful.
        /// </summary>
        protected virtual void CheckModel(
            ModelBindingContext bindingContext,
            ValueProviderResult valueProviderResult,
            object? model)
        {
            // When converting newModel a null value may indicate a failed conversion for an otherwise required
            // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
            // current bindingContext. If not, an error is logged.
            if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                        valueProviderResult.ToString()));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }



        /*
         
        if (!Guid.TryParse(valueProviderResult?.FirstValue, out Guid result))
            {
                if (bindingContext?.ModelType == typeof(Guid?))
                {
                    bindingContext.Result = ModelBindingResult.Success(new Guid?());
                    return Task.CompletedTask;
                }
                result = Guid.Empty;
            }

            if (bindingContext != null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }
            else
                return Task.FromResult(result);
         
         */
    }
}
