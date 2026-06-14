using CadastroVeiculos.Maui.Models;

namespace CadastroVeiculos.Maui.Services;

public class AuthService
{
    private const string TokenKey = "auth_token";
    private const string TokenExpirationKey = "auth_token_expiration";

    public string? ObterToken()
    {
        return Preferences.Get(TokenKey, null);
    }

    public bool EstaAutenticado()
    {
        var token = ObterToken();
        if (string.IsNullOrEmpty(token))
            return false;

        var expiration = Preferences.Get(TokenExpirationKey, string.Empty);
        if (DateTime.TryParse(expiration, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expirationDate))
            return expirationDate > DateTime.UtcNow;

        return false;
    }

    public void SalvarToken(LoginResponse loginResponse)
    {
        Preferences.Set(TokenKey, loginResponse.Token);

        if (DateTime.TryParse(loginResponse.ExpiresIn, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var expirationDate))
        {
            Preferences.Set(TokenExpirationKey, expirationDate.ToString("O"));
        }
    }

    public async Task Logout()
    {
        Preferences.Remove(TokenKey);
        Preferences.Remove(TokenExpirationKey);

        if (Application.Current?.Windows.Count > 0 && Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
