using System.Collections.ObjectModel;
using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroVeiculos.Maui.ViewModels;

public partial class VeiculosListaViewModel : ObservableObject
{
    private readonly VeiculoService _veiculoService;

    public VeiculosListaViewModel(VeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [ObservableProperty]
    public partial ObservableCollection<VeiculoResponse> Veiculos { get; set; } = [];

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool FiltroVisivel { get; set; }

    [ObservableProperty]
    public partial string FiltroDescricao { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MarcaItem? FiltroMarca { get; set; }

    [ObservableProperty]
    public partial string FiltroModelo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FiltroOpcionais { get; set; } = string.Empty;

    public List<MarcaItem> Marcas { get; } = [new MarcaItem { Id = 0, Nome = "Todas" }, .. MarcaItem.ObterTodas()];

    public bool TemFiltroAtivo =>
        !string.IsNullOrWhiteSpace(FiltroDescricao) ||
        (FiltroMarca != null && FiltroMarca.Id > 0) ||
        !string.IsNullOrWhiteSpace(FiltroModelo) ||
        !string.IsNullOrWhiteSpace(FiltroOpcionais);

    [RelayCommand]
    private async Task CarregarVeiculos()
    {
        try
        {
            Carregando = true;
            Erro = string.Empty;

            List<VeiculoResponse> lista;

            if (TemFiltroAtivo)
            {
                lista = await _veiculoService.Buscar(
                    FiltroDescricao,
                    FiltroMarca?.Id > 0 ? FiltroMarca.Id : null,
                    FiltroModelo,
                    FiltroOpcionais);
            }
            else
            {
                lista = await _veiculoService.Listar();
            }

            Veiculos = new ObservableCollection<VeiculoResponse>(lista);
        }
        catch (UnauthorizedAccessException)
        {
            // AuthService handles redirect
        }
        catch (Exception)
        {
            Erro = "Erro ao carregar veiculos.";
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private void AlternarFiltro()
    {
        FiltroVisivel = !FiltroVisivel;
    }

    [RelayCommand]
    private async Task Pesquisar()
    {
        OnPropertyChanged(nameof(TemFiltroAtivo));
        await CarregarVeiculos();
    }

    [RelayCommand]
    private async Task LimparFiltros()
    {
        FiltroDescricao = string.Empty;
        FiltroMarca = null;
        FiltroModelo = string.Empty;
        FiltroOpcionais = string.Empty;
        OnPropertyChanged(nameof(TemFiltroAtivo));
        await CarregarVeiculos();
    }

    [RelayCommand]
    private async Task Adicionar()
    {
        await Shell.Current.GoToAsync("veiculoForm");
    }

    [RelayCommand]
    private async Task Editar(VeiculoResponse veiculo)
    {
        await Shell.Current.GoToAsync($"veiculoForm?id={veiculo.Id}");
    }

    [RelayCommand]
    private async Task Excluir(VeiculoResponse veiculo)
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar Exclusao",
            $"Deseja realmente excluir o veiculo \"{veiculo.Descricao}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            await _veiculoService.Excluir(veiculo.Id);
            Veiculos.Remove(veiculo);

            await Shell.Current.DisplayAlertAsync("Sucesso", "Veiculo excluido com sucesso!", "OK");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao excluir veiculo.", "OK");
        }
    }
}
