using System.Collections.ObjectModel;
using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroVeiculos.Maui.ViewModels;

public partial class UsuariosListaViewModel : ObservableObject
{
    private readonly UsuarioService _usuarioService;

    public UsuariosListaViewModel(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [ObservableProperty]
    public partial ObservableCollection<UsuarioResponse> Usuarios { get; set; } = [];

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [RelayCommand]
    private async Task CarregarUsuarios()
    {
        try
        {
            Carregando = true;
            Erro = string.Empty;

            var lista = await _usuarioService.Listar();
            Usuarios = new ObservableCollection<UsuarioResponse>(lista);
        }
        catch (UnauthorizedAccessException)
        {
            // AuthService handles redirect
        }
        catch (Exception)
        {
            Erro = "Erro ao carregar usuarios.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task Editar(UsuarioResponse usuario)
    {
        await Shell.Current.GoToAsync($"usuarioForm?id={usuario.Id}");
    }

    [RelayCommand]
    private async Task Excluir(UsuarioResponse usuario)
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar Exclusao",
            $"Deseja realmente excluir o usuario \"{usuario.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            await _usuarioService.Excluir(usuario.Id);
            Usuarios.Remove(usuario);

            await Shell.Current.DisplayAlertAsync("Sucesso", "Usuario excluido com sucesso!", "OK");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao excluir usuario.", "OK");
        }
    }
}
