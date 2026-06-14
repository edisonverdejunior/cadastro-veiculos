using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class UsuariosListaPage : ContentPage
{
    private readonly UsuariosListaViewModel _viewModel;

    public UsuariosListaPage(UsuariosListaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.CarregarUsuariosCommand.ExecuteAsync(null);
        }
        catch (Exception)
        {
        }
    }
}
