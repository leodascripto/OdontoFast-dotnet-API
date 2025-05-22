// OdontofastAPI/Configuration/MLNetServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OdontofastAPI.ML;
using OdontofastAPI.Service.Implementations.ML;
using OdontofastAPI.Service.Interfaces.ML;
using System;

namespace OdontofastAPI.Configuration
{
  /// <summary>
  /// Extensões para configuração dos serviços de ML.NET
  /// </summary>
  public static class MLNetServiceExtensions
  {
    /// <summary>
    /// Adiciona serviços relacionados ao ML.NET à coleção de serviços
    /// </summary>
    public static IServiceCollection AddMLNetServices(this IServiceCollection services)
    {
      // Adiciona serviços como AddScoped para manter modelos em memória
      services.AddScoped<IModelManagerService, ModelManagerService>();
      services.AddScoped<IPreditorTratamentoService, PreditorTratamentoService>();
      services.AddScoped<IRecomendadorService, RecomendadorService>();

      return services;
    }

    /// <summary>
    /// Inicializa os modelos de ML.NET (deve ser chamado após a configuração do aplicativo)
    /// </summary>
    public static void InitializeMLModels(this IServiceProvider serviceProvider, bool forceTraining = false)
    {
      try
      {
        // Cria um escopo para acessar serviços Scoped
        using (var scope = serviceProvider.CreateScope())
        {
          var scopedProvider = scope.ServiceProvider;

          var logger = scopedProvider.GetRequiredService<ILogger<ModelTrainer>>();
          var modelManager = scopedProvider.GetRequiredService<IModelManagerService>();

          // Verifica se os modelos existem
          var tratamentoDuracaoExists = modelManager.VerificarModeloExisteAsync("TratamentoDuracao").GetAwaiter().GetResult();

          // Resto do código permanece igual...
          // Apenas certifique-se de usar scopedProvider ao invés de serviceProvider
        }
      }
      catch (Exception ex)
      {
        // Aqui precisamos obter o logger do serviceProvider original (não do escopo)
        // já que estamos no catch e o escopo pode não estar mais disponível
        var logger = serviceProvider.GetRequiredService<ILogger<ModelTrainer>>();
        logger.LogError(ex, "Erro ao inicializar modelos de ML.NET");
      }
    }
  }
}
