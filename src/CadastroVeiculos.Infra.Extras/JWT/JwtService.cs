using CadastroVeiculos.Infra.Extras.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroVeiculos.Infra.Extras.JWT
{
    public static class JwtService
    {
        public static string GenerateToken(Guid userId, string login)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetSectionValue("Jwt", "Key")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("sub", userId.ToString()),
                new Claim("login", login),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, login),
                new Claim("role", "User")
            };
            var token = new JwtSecurityToken(
                issuer: Config.GetSectionValue("Jwt", "Issuer"),
                audience: Config.GetSectionValue("Jwt", "Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
