using CadastroVeiculos.Maui.Models;
using CadastroVeiculos.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroVeiculos.Maui.ViewModels;

[QueryProperty(nameof(VeiculoId), "id")]
public partial class VeiculoFormViewModel : ObservableObject
{
    private readonly VeiculoService _veiculoService;

    public VeiculoFormViewModel(VeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    private string? _veiculoId;
    public string? VeiculoId
    {
        get => _veiculoId;
        set
        {
            _veiculoId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(EhEdicao));
            if (!string.IsNullOrEmpty(value))
                _ = CarregarVeiculo(value);
        }
    }

    public bool EhEdicao => !string.IsNullOrEmpty(VeiculoId);
    public string Titulo => EhEdicao ? "Editar Veiculo" : "Novo Veiculo";

    [ObservableProperty]
    public partial string Descricao { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MarcaItem? MarcaSelecionada { get; set; }

    [ObservableProperty]
    public partial string Modelo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Opcionais { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Valor { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Erro { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Carregando { get; set; }

    public List<MarcaItem> Marcas { get; } = MarcaItem.ObterTodas();

    private async Task CarregarVeiculo(string id)
    {
        try
        {
            Carregando = true;
            var veiculo = await _veiculoService.Obter(id);
            if (veiculo != null)
            {
                Descricao = veiculo.Descricao;
                MarcaSelecionada = Marcas.Find(m => m.Id == veiculo.Marca);
                Modelo = veiculo.Modelo;
                Opcionais = veiculo.Opcionais ?? string.Empty;
                Valor = veiculo.Valor?.ToString("F2") ?? string.Empty;
            }
        }
        catch (Exception)
        {
            Erro = "Erro ao carregar veiculo.";
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

        if (string.IsNullOrWhiteSpace(Descricao))
        {
            Erro = "Informe a descricao.";
            return;
        }

        if (MarcaSelecionada == null)
        {
            Erro = "Selecione a marca.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Modelo))
        {
            Erro = "Informe o modelo.";
            return;
        }

        decimal? valorDecimal = null;
        if (!string.IsNullOrWhiteSpace(Valor))
        {
            if (!decimal.TryParse(Valor, out var v) || v <= 0)
            {
                Erro = "Valor deve ser um numero positivo.";
                return;
            }
            valorDecimal = v;
        }

        try
        {
            Carregando = true;

            var request = new VeiculoRequest
            {
                Descricao = Descricao,
                Marca = MarcaSelecionada.Id,
                Modelo = Modelo,
                Opcionais = string.IsNullOrWhiteSpace(Opcionais) ? null : Opcionais,
                Valor = valorDecimal
            };

            if (EhEdicao)
                await _veiculoService.Atualizar(VeiculoId!, request);
            else
                await _veiculoService.Criar(request);

            await Shell.Current.DisplayAlertAsync("Sucesso",
                EhEdicao ? "Veiculo atualizado com sucesso!" : "Veiculo cadastrado com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            Erro = "Erro ao salvar veiculo.";
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

    public void LimparFormulario()
    {
        _veiculoId = null;
        Descricao = string.Empty;
        MarcaSelecionada = null;
        Modelo = string.Empty;
        Opcionais = string.Empty;
        Valor = string.Empty;
        Erro = string.Empty;
        OnPropertyChanged(nameof(VeiculoId));
        OnPropertyChanged(nameof(Titulo));
        OnPropertyChanged(nameof(EhEdicao));
    }
}
