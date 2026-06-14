using CadastroVeiculos.Maui.ViewModels;

namespace CadastroVeiculos.Maui.Views;

public partial class VeiculoFormPage : ContentPage
{
    private readonly VeiculoFormViewModel _viewModel;

    public VeiculoFormPage(VeiculoFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (string.IsNullOrEmpty(_viewModel.VeiculoId))
            _viewModel.LimparFormulario();
    }
}
