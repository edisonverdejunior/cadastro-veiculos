using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroVeiculos.Maui.ViewModels;

public partial class CadastroViewModel : ObservableObject
{
    private readonly UsuarioService _usuarioService;

    public CadastroViewModel(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Sucesso { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    [RelayCommand]
    private async Task Cadastrar()
    {
        Erro = string.Empty;
        Sucesso = string.Empty;

        if (string.IsNullOrWhiteSpace(Nome))
        {
            Erro = "Informe o nome.";
            return;
        }

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

            var request = new CadastrarUsuarioRequest
            {
                Nome = Nome,
                Login = Login,
                Senha = Senha
            };

            await _usuarioService.Criar(request);

            Sucesso = "Conta criada com sucesso! Redirecionando...";
            Nome = string.Empty;
            Login = string.Empty;
            Senha = string.Empty;

            await Task.Delay(1500);
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception)
        {
            Erro = "Erro ao criar conta. Tente novamente.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task IrParaLogin()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
