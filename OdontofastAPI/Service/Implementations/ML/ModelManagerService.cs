// OdontofastAPI/Service/Implementations/ML/ModelManagerService.cs
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using OdontofastAPI.Service.Interfaces.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations.ML
{
  /// <summary>
  /// Implementação do serviço de gerenciamento de modelos ML
  /// </summary>
  public class ModelManagerService : IModelManagerService
  {
    private readonly ILogger<ModelManagerService> _logger;
    private readonly MLContext _mlContext;
    private readonly string _modelDirectory;
    private readonly Dictionary<string, ITransformer> _loadedModels;

    public ModelManagerService(ILogger<ModelManagerService> logger)
    {
      _logger = logger;
      _mlContext = new MLContext(seed: 42); // Seed fixo para reprodutibilidade
      _modelDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ML", "Models");
      _loadedModels = new Dictionary<string, ITransformer>();

      // Garante que o diretório existe
      if (!Directory.Exists(_modelDirectory))
      {
        Directory.CreateDirectory(_modelDirectory);
        _logger.LogInformation($"Diretório de modelos criado em {_modelDirectory}");
      }
    }

    /// <inheritdoc/>
    public async Task<bool> CarregarModeloAsync<TInput, TOutput>(string modelName)
        where TInput : class
        where TOutput : class, new()
    {
      try
      {
        // Se o modelo já estiver em memória, não precisamos carregá-lo novamente
        if (_loadedModels.ContainsKey(modelName))
        {
          _logger.LogInformation($"Modelo {modelName} já está carregado em memória");
          return true;
        }

        string modelPath = Path.Combine(_modelDirectory, $"{modelName}.zip");

        // Verifica se o arquivo existe
        if (!File.Exists(modelPath))
        {
          _logger.LogWarning($"Modelo {modelName} não encontrado em {modelPath}");
          return false;
        }

        // Carregamento é síncrono, mas envolvemos em Task para manter a assinatura assíncrona
        await Task.Run(() =>
        {
          // Carrega o modelo do arquivo
          var model = _mlContext.Model.Load(modelPath, out var _);
          // Armazena o modelo em cache
          _loadedModels[modelName] = model;
        });

        _logger.LogInformation($"Modelo {modelName} carregado com sucesso");
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Erro ao carregar modelo {modelName}");
        return false;
      }
    }

    /// <inheritdoc/>
    public async Task<bool> SalvarModeloAsync<TInput, TOutput>(string modelName, ITransformer model, DataViewSchema schema)
        where TInput : class
        where TOutput : class, new()
    {
      try
      {
        if (model == null)
        {
          _logger.LogError($"Tentativa de salvar modelo nulo para {modelName}");
          return false;
        }

        string modelPath = Path.Combine(_modelDirectory, $"{modelName}.zip");

        // Salvamento é síncrono, mas envolvemos em Task para manter a assinatura assíncrona
        await Task.Run(() =>
        {
          // Salva o modelo no arquivo
          _mlContext.Model.Save(model, schema, modelPath);
          // Atualiza o cache em memória
          _loadedModels[modelName] = model;
        });

        _logger.LogInformation($"Modelo {modelName} salvo com sucesso em {modelPath}");
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Erro ao salvar modelo {modelName}");
        return false;
      }
    }

    /// <inheritdoc/>
    public async Task<bool> VerificarModeloExisteAsync(string modelName)
    {
      try
      {
        // Verifica se o modelo está no cache em memória
        if (_loadedModels.ContainsKey(modelName))
        {
          return true;
        }

        // Verifica se o arquivo existe no disco
        string modelPath = Path.Combine(_modelDirectory, $"{modelName}.zip");
        bool exists = File.Exists(modelPath);

        await Task.CompletedTask; // Para manter a assinatura assíncrona
        return exists;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Erro ao verificar existência do modelo {modelName}");
        return false;
      }
    }

    /// <summary>
    /// Obtém um modelo carregado em memória
    /// </summary>
    /// <param name="modelName">Nome do modelo</param>
    /// <returns>O modelo carregado ou null se não existir</returns>
    public ITransformer? GetLoadedModel(string modelName)
    {
      if (_loadedModels.TryGetValue(modelName, out var model))
      {
        return model;
      }
      return null;
    }

    /// <summary>
    /// Cria um motor de predição para o modelo especificado
    /// </summary>
    /// <typeparam name="TInput">Tipo de entrada do modelo</typeparam>
    /// <typeparam name="TOutput">Tipo de saída do modelo</typeparam>
    /// <param name="modelName">Nome do modelo</param>
    /// <returns>Motor de predição ou null se o modelo não estiver carregado</returns>
    public PredictionEngine<TInput, TOutput>? CreatePredictionEngine<TInput, TOutput>(string modelName)
        where TInput : class
        where TOutput : class, new()
    {
      if (_loadedModels.TryGetValue(modelName, out var model))
      {
        return _mlContext.Model.CreatePredictionEngine<TInput, TOutput>(model);
      }
      _logger.LogWarning($"Tentativa de criar PredictionEngine para modelo não carregado: {modelName}");
      return null;
    }

    /// <summary>
    /// Remove um modelo do cache em memória
    /// </summary>
    /// <param name="modelName">Nome do modelo</param>
    /// <returns>True se o modelo foi removido, False caso contrário</returns>
    public bool UnloadModel(string modelName)
    {
      if (_loadedModels.ContainsKey(modelName))
      {
        _loadedModels.Remove(modelName);
        _logger.LogInformation($"Modelo {modelName} removido do cache");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Lista todos os modelos disponíveis (tanto em memória quanto em disco)
    /// </summary>
    /// <returns>Lista de nomes de modelos</returns>
    public async Task<List<string>> ListarModelosDisponiveisAsync()
    {
      try
      {
        var modelos = new HashSet<string>();

        // Adiciona modelos em memória
        foreach (var modelo in _loadedModels.Keys)
        {
          modelos.Add(modelo);
        }

        // Adiciona modelos em disco
        if (Directory.Exists(_modelDirectory))
        {
          foreach (var arquivo in Directory.GetFiles(_modelDirectory, "*.zip"))
          {
            var nomeModelo = Path.GetFileNameWithoutExtension(arquivo);
            modelos.Add(nomeModelo);
          }
        }

        await Task.CompletedTask; // Para manter a assinatura assíncrona
        return new List<string>(modelos);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao listar modelos disponíveis");
        return new List<string>();
      }
    }
  }
}