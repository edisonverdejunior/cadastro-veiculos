using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using System.Text.Json;

namespace CadastroVeiculos.Maui.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LoginViewModel(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    [ObservableProperty]
    public partial bool MostrarSenha { get; set; }

    [RelayCommand]
    private async Task Entrar()
    {
        Erro = string.Empty;

        if (string.IsNullOrWhiteSpace(Login))
        {
            Erro = "Informe o login.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Senha) || Senha.Length < 6)
        {
            Erro = "A senha deve ter pelo menos 6 caracteres.";
            return;
        }

        try
        {
            Carregando = true;

            var request = new LoginRequest { Login = Login, Senha = Senha };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                Erro = "Login ou senha incorretos.";
                return;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
            if (loginResponse != null)
            {
                _authService.SalvarToken(loginResponse);
                Login = string.Empty;
                Senha = string.Empty;
                await Shell.Current.GoToAsync("//main/tab_veiculos/veiculos");
            }
        }
        catch (Exception ex)
        {
            Erro = "Erro ao conectar com o servidor. Verifique sua conexao.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task IrParaCadastro()
    {
        await Shell.Current.GoToAsync("//cadastro");
    }

    [RelayCommand]
    private void ToggleMostrarSenha()
    {
        MostrarSenha = !MostrarSenha;
    }
}
