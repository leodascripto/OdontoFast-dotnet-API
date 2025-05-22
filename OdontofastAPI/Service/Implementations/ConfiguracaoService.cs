using Microsoft.Extensions.Configuration;
using OdontofastAPI.Service.Interfaces;

namespace OdontofastAPI.Service.Implementations
{
    public class ConfiguracaoService : IConfiguracaoService
    {
        private static ConfiguracaoService? _instance;
        private readonly IConfiguration _configuration;

        private ConfiguracaoService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static ConfiguracaoService GetInstance(IConfiguration configuration)
        {
            if (_instance == null)
            {
                _instance = new ConfiguracaoService(configuration);
            }
            return _instance;
        }

        public string? GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }
    }
}