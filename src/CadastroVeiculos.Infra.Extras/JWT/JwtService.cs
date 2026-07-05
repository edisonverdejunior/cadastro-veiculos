using CadastroVeiculos.Infra.Extras.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroVeiculos.Infra.Extras.JWT
{
    public static class JwtService
    {
        /// <summary>Audience exclusiva do pré-auth token de MFA. Não é aceita pelos endpoints normais.</summary>
        public const string PreAuthAudience = "CadastroVeiculosMfaPending";

        private const int PreAuthMinutes = 5;

        private static ILogger? _logger;

        /// <summary>Inicializa o logger estático. Deve ser chamado em Program.cs após builder.Build().</summary>
        public static void Initialize(ILoggerFactory loggerFactory)
            => _logger = loggerFactory.CreateLogger(nameof(JwtService));

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
                new Claim("role", "User"),
                new Claim("amr", "mfa")
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

        /// <summary>
        /// Token curto (5 min) emitido entre a etapa de senha e a etapa de TOTP.
        /// Usa a mesma chave/issuer, mas audience própria (<see cref="PreAuthAudience"/>),
        /// de modo que é rejeitado por qualquer endpoint [Authorize] normal.
        /// </summary>
        public static string GeneratePreAuthToken(Guid userId, string login)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetSectionValue("Jwt", "Key")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("sub", userId.ToString()),
                new Claim("login", login),
                new Claim("mfa", "pending")
            };
            var token = new JwtSecurityToken(
                issuer: Config.GetSectionValue("Jwt", "Issuer"),
                audience: PreAuthAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(PreAuthMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>Valida um pré-auth token de MFA e devolve o userId, ou null se inválido/expirado.</summary>
        public static Guid? ValidatePreAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetSectionValue("Jwt", "Key")));
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Config.GetSectionValue("Jwt", "Issuer"),
                ValidAudience = PreAuthAudience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            try
            {
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out var validated);

                if (validated is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                    return null;

                if (principal.FindFirst("mfa")?.Value != "pending")
                    return null;

                var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(sub, out var userId) ? userId : null;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger?.LogWarning("Pré-auth token MFA expirado em {Expiry:O}.", ex.Expires);
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger?.LogWarning(ex, "Pré-auth token MFA rejeitado ({ExceptionType}).", ex.GetType().Name);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao validar pré-auth token MFA.");
                return null;
            }
        }

        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtKey = Config.GetSectionValue("Jwt", "Key");
            var jwtIssuer = Config.GetSectionValue("Jwt", "Issuer");
            var jwtAudience = Config.GetSectionValue("Jwt", "Audience");

            if (!string.IsNullOrWhiteSpace(jwtKey))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtIssuer,
                            ValidAudience = jwtAudience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                        };
                    });
            }
        }
    }
}
