using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class UsuarioFormPage : ContentPage
{
    public UsuarioFormPage(UsuarioFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
