using System.Net;
using System.Text;

namespace CadastroVeiculos.API.Middlewares
{
    public class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment env)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;

                string errorMessage = env.IsProduction() 
                    ? "An error occurred while processing your request." 
                    : ex.ToString();

                byte[] buffer = Encoding.UTF8.GetBytes(errorMessage);
                httpContext.Response.ContentLength = buffer.Length;
                await httpContext.Response.Body.WriteAsync(buffer);

                logger.LogError(ex, ex.Message);
            }
        }
    }
}
