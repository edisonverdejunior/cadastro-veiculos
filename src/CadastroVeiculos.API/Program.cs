using CadastroVeiculos.API.Configurations;
using CadastroVeiculos.API.Middlewares;
using CadastroVeiculos.Infra.Data.Context;
using CadastroVeiculos.Infra.Extras.IoC;
using CadastroVeiculos.Infra.Extras.JWT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();

builder.Services.RegisterAllDependencies();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAnyOriginAnyMethod",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CadastroVeiculosContext>();
    context.SeedInMemoryData();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cadastro de Veículos API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAnyOriginAnyMethod");
app.MapControllers();

app.Run("https://0.0.0.0:5217");
//app.Run();
