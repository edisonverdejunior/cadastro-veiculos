namespace CadastroVeiculos.Maui.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string ExpiresIn { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
}
