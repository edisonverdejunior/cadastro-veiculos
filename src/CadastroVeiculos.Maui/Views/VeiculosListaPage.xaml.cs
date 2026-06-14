using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class VeiculosListaPage : ContentPage
{
    private readonly VeiculosListaViewModel _viewModel;

    public VeiculosListaPage(VeiculosListaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.CarregarVeiculosCommand.ExecuteAsync(null);
        }
        catch (Exception)
        {
        }
    }
}
