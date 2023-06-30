using System.Security.Authentication;

namespace webapi.Framework
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is AuthenticationException)
                {
                    // Escreve as informações de log correspondentes aqui
                    _logger.LogError(ex, "Ocorreu um erro não tratado.");
                }

                throw;
            }
        }
    }
}
