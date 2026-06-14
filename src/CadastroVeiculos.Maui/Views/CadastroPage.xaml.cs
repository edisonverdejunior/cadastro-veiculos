using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class CadastroPage : ContentPage
{
    public CadastroPage(CadastroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
