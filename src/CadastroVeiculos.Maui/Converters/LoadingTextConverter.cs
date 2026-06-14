using System.Globalization;

namespace CadastroVeiculos.Maui.Converters;

public class LoadingTextConverter : IValueConverter
{
    public string DefaultText { get; set; } = "Entrar";
    public string LoadingText { get; set; } = "Entrando...";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? LoadingText : DefaultText;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
