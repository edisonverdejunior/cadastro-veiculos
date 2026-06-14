using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void EntryLogin_Completed(object? sender, EventArgs e)
    {
        EntrySenha.Focus();
    }
}
