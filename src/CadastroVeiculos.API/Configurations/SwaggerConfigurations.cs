using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;


namespace CadastroVeiculos.API.Configurations
{
    public static class SwaggerConfigurations
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Cadastro de Veículos API", 
                    Version = "v1", 
                    Description = "API para cadastro de veículos"
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Informe **_SOMENTE_** o seu JWT Bearer token na caixa de texto abaixo!",
                };

                option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
                
            });
        }
    }
}
