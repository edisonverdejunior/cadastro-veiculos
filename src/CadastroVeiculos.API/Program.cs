using CadastroVeiculos.API.Configurations;
using CadastroVeiculos.API.Middlewares;
using CadastroVeiculos.Infra.Data.Context;
using CadastroVeiculos.Infra.Extras.IoC;
using CadastroVeiculos.Infra.Extras.JWT;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();

// Chaves do Data Protection persistidas em disco para que o MfaSecret cifrado
// continue legível após reinícios da aplicação.
builder.Services.AddDataProtection()
    .SetApplicationName("CadastroVeiculos")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "dataprotection-keys")));

builder.Services.RegisterAllDependencies(builder.Configuration);

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

JwtService.Initialize(app.Services.GetRequiredService<ILoggerFactory>());

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CadastroVeiculosContext>();
    context.Database.Migrate();
    context.SeedData();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cadastro de Ve�culos API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAnyOriginAnyMethod");
app.MapControllers();

//app.Run("https://0.0.0.0:5217");
app.Run();
