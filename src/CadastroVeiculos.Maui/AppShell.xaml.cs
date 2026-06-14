using CadastroVeiculos.Maui.Services;
using CadastroVeiculos.Maui.Views;

namespace CadastroVeiculos.Maui;

public partial class AppShell : Shell
{
    private readonly AuthService _authService;

    public AppShell(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        Routing.RegisterRoute("veiculoForm", typeof(VeiculoFormPage));
        Routing.RegisterRoute("usuarioForm", typeof(UsuarioFormPage));
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        try
        {
            await _authService.Logout();
        }
        catch (Exception)
        {
        }
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        try
        {
            var currentRoute = Current?.CurrentState?.Location?.ToString();

            if (string.IsNullOrEmpty(currentRoute))
                return;

            if (currentRoute.Contains("login") || currentRoute.Contains("cadastro"))
                return;

            if (!_authService.EstaAutenticado())
            {
                await GoToAsync("//login");
            }
        }
        catch (Exception)
        {
        }
    }
}
