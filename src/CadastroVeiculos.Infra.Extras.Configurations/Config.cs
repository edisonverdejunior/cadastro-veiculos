using Microsoft.Extensions.Configuration;

namespace CadastroVeiculos.Infra.Extras.Configurations
{
    public static class Config
    {
        private static IConfiguration _configuration;

        public static IConfiguration Conf
        {
            get
            {
                if (_configuration == null)
                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                return _configuration;
            }
        }

        public static string? GetSectionValue(string section, string value)
            => Conf.GetSection(section).GetValue<string>(value);
    }
}
