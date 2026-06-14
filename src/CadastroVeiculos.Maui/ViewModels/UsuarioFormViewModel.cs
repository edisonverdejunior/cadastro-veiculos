using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroVeiculos.Maui.ViewModels;

[QueryProperty(nameof(UsuarioId), "id")]
public partial class UsuarioFormViewModel : ObservableObject
{
    private readonly UsuarioService _usuarioService;

    public UsuarioFormViewModel(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    private string? _usuarioId;
    public string? UsuarioId
    {
        get => _usuarioId;
        set
        {
            _usuarioId = value;
            OnPropertyChanged();
            if (!string.IsNullOrEmpty(value))
                _ = CarregarUsuario(value);
        }
    }

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string LoginUsuario { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    private async Task CarregarUsuario(string id)
    {
        try
        {
            Carregando = true;
            var usuario = await _usuarioService.Obter(id);
            if (usuario != null)
            {
                Nome = usuario.Nome;
                LoginUsuario = usuario.Login;
            }
        }
        catch (Exception)
        {
            Erro = "Erro ao carregar usuario.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task Salvar()
    {
        Erro = string.Empty;

        if (string.IsNullOrWhiteSpace(Nome))
        {
            Erro = "Informe o nome.";
            return;
        }

        try
        {
            Carregando = true;

            var request = new AtualizarUsuarioRequest { Nome = Nome };
            await _usuarioService.Atualizar(UsuarioId!, request);

            await Shell.Current.DisplayAlertAsync("Sucesso", "Usuario atualizado com sucesso!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            Erro = "Erro ao salvar usuario.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task Voltar()
    {
        await Shell.Current.GoToAsync("..");
    }
}
