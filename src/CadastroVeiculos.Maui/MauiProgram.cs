using CadastroVeiculos.Maui.Services;
using CadastroVeiculos.Maui.ViewModels;
using CadastroVeiculos.Maui.Views;
using Microsoft.Extensions.Logging;

namespace CadastroVeiculos.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBackground", (handler, view) =>
                {
#if ANDROID
                    handler.PlatformView.BackgroundTintList =
                        Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
                });
            });

        // HttpClient
        builder.Services.AddSingleton(sp =>
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var client = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri("https://192.168.0.83:5217/")
            };

            return client;
        });

        // Shell
        builder.Services.AddSingleton<AppShell>();

        // Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddSingleton<VeiculoService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CadastroViewModel>();
        builder.Services.AddTransient<VeiculosListaViewModel>();
        builder.Services.AddTransient<VeiculoFormViewModel>();
        builder.Services.AddTransient<UsuariosListaViewModel>();
        builder.Services.AddTransient<UsuarioFormViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CadastroPage>();
        builder.Services.AddTransient<VeiculosListaPage>();
        builder.Services.AddTransient<VeiculoFormPage>();
        builder.Services.AddTransient<UsuariosListaPage>();
        builder.Services.AddTransient<UsuarioFormPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
