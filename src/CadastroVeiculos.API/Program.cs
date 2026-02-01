using CadastroVeiculos.API.Configurations;
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CadastroVeiculosContext>();
    context.SeedInMemoryData();
}

// Configure the HTTP request pipeline.
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

app.MapControllers();

app.Run();
