// OdontofastAPI/ML/ModelTrainer.cs
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using OdontofastAPI.Model.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OdontofastAPI.ML
{
  /// <summary>
  /// Classe utilitária para treinamento de modelos ML.NET
  /// </summary>
  public class ModelTrainer
  {
    private readonly ILogger<ModelTrainer> _logger;
    private readonly MLContext _mlContext;
    private readonly string _modelDirectory;

    public ModelTrainer(ILogger<ModelTrainer> logger)
    {
      _logger = logger;
      _mlContext = new MLContext(seed: 42);
      _modelDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ML", "Models");

      // Garante que o diretório existe
      if (!Directory.Exists(_modelDirectory))
      {
        Directory.CreateDirectory(_modelDirectory);
      }
    }

    /// <summary>
    /// Treina e salva o modelo de predição de duração de tratamento
    /// </summary>
    public void TrainAndSaveTratamentoDuracaoModel()
    {
      try
      {
        _logger.LogInformation("Iniciando treinamento do modelo de duração de tratamento");

        // Dados de treinamento - em produção, viria do banco de dados
        var trainingData = GenerateTratamentoDuracaoSampleData();

        // Carrega os dados para o formato do ML.NET
        var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

        // Define o pipeline de treinamento
        var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TratamentoDuracaoInput.ComplexidadeTratamento))
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "TipoTratamentoEncoded", inputColumnName: nameof(TratamentoDuracaoInput.TipoTratamento)))
            .Append(_mlContext.Transforms.Concatenate("Features",
                nameof(TratamentoDuracaoInput.Idade),
                "TipoTratamentoEncoded",
                nameof(TratamentoDuracaoInput.ComplexidadeTratamento),
                nameof(TratamentoDuracaoInput.PossuiComorbidades),
                nameof(TratamentoDuracaoInput.IndiceSaude),
                nameof(TratamentoDuracaoInput.TaxaAdesaoAnterior)))
            .Append(_mlContext.Regression.Trainers.FastTree());

        // Treina o modelo
        var model = pipeline.Fit(trainingDataView);

        // Avalia o modelo (opcional)
        var predictions = model.Transform(trainingDataView);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        _logger.LogInformation($"Modelo treinado com R²: {metrics.RSquared:F2}");

        // Salva o modelo
        var modelPath = Path.Combine(_modelDirectory, "TratamentoDuracao.zip");
        _mlContext.Model.Save(model, trainingDataView.Schema, modelPath);

        _logger.LogInformation($"Modelo salvo em {modelPath}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao treinar modelo de duração de tratamento");
        throw;
      }
    }

    /// <summary>
    /// Gera dados de amostra para treinar o modelo de duração de tratamento
    /// </summary>
    /// <remarks>
    /// Em produção, esses dados seriam extraídos de casos reais do banco de dados
    /// </remarks>
    private IEnumerable<TratamentoDuracaoInput> GenerateTratamentoDuracaoSampleData()
    {
      // Dados de exemplo para diferentes tipos de tratamento e níveis de complexidade
      var random = new Random(42);
      var data = new List<TratamentoDuracaoInput>();

      // Ortodontia - geralmente de longa duração
      for (int i = 0; i < 50; i++)
      {
        data.Add(new TratamentoDuracaoInput
        {
          Idade = random.Next(10, 60),
          TipoTratamento = "Ortodontia",
          ComplexidadeTratamento = random.Next(1, 6), // 1-5 escala de complexidade
          PossuiComorbidades = random.Next(0, 10) < 3, // 30% chance de comorbidades
          IndiceSaude = (float)random.NextDouble() * 0.5f + 0.5f, // 0.5-1.0
          TaxaAdesaoAnterior = (float)random.NextDouble() * 0.3f + 0.7f // 0.7-1.0
        });
      }

      // Implante - duração média
      for (int i = 0; i < 50; i++)
      {
        data.Add(new TratamentoDuracaoInput
        {
          Idade = random.Next(30, 80),
          TipoTratamento = "Implante",
          ComplexidadeTratamento = random.Next(1, 6),
          PossuiComorbidades = random.Next(0, 10) < 4, // 40% chance
          IndiceSaude = (float)random.NextDouble() * 0.6f + 0.4f, // 0.4-1.0
          TaxaAdesaoAnterior = (float)random.NextDouble() * 0.4f + 0.6f // 0.6-1.0
        });
      }

      // Canal - duração curta
      for (int i = 0; i < 50; i++)
      {
        data.Add(new TratamentoDuracaoInput
        {
          Idade = random.Next(20, 70),
          TipoTratamento = "Canal",
          ComplexidadeTratamento = random.Next(1, 6),
          PossuiComorbidades = random.Next(0, 10) < 2, // 20% chance
          IndiceSaude = (float)random.NextDouble() * 0.7f + 0.3f, // 0.3-1.0
          TaxaAdesaoAnterior = (float)random.NextDouble() * 0.5f + 0.5f // 0.5-1.0
        });
      }

      // Limpeza - muito curta duração
      for (int i = 0; i < 50; i++)
      {
        data.Add(new TratamentoDuracaoInput
        {
          Idade = random.Next(10, 80),
          TipoTratamento = "Limpeza",
          ComplexidadeTratamento = random.Next(1, 3), // Menor complexidade
          PossuiComorbidades = random.Next(0, 10) < 1, // 10% chance
          IndiceSaude = (float)random.NextDouble() * 0.8f + 0.2f, // 0.2-1.0
          TaxaAdesaoAnterior = (float)random.NextDouble() * 0.7f + 0.3f // 0.3-1.0
        });
      }

      return data;
    }

  }

}