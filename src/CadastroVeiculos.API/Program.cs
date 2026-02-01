using CadastroVeiculos.API.Configurations;
using CadastroVeiculos.Infra.Extras.IoC;
using CadastroVeiculos.Infra.Extras.JWT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

builder.Services.RegisterAllDependencies();

var app = builder.Build();

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
