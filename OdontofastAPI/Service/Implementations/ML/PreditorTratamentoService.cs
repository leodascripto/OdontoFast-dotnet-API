// OdontofastAPI/Service/Implementations/ML/PreditorTratamentoService.cs
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using OdontofastAPI.DTO.ML;
using OdontofastAPI.Model.ML;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Interfaces.ML;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations.ML
{
  /// <summary>
  /// Implementação do serviço de predição de duração de tratamentos
  /// </summary>
  public class PreditorTratamentoService : IPreditorTratamentoService
  {
    private readonly ILogger<PreditorTratamentoService> _logger;
    private readonly MLContext _mlContext;
    private readonly IModelManagerService _modelManagerService;
    private readonly IUsuarioRepository _usuarioRepository;
    private PredictionEngine<TratamentoDuracaoInput, TratamentoDuracaoOutput>? _predictionEngine;
    private const string MODEL_NAME = "TratamentoDuracao";

    public PreditorTratamentoService(
        ILogger<PreditorTratamentoService> logger,
        IModelManagerService modelManagerService,
        IUsuarioRepository usuarioRepository)
    {
      _logger = logger;
      _mlContext = new MLContext(seed: 42);
      _modelManagerService = modelManagerService;
      _usuarioRepository = usuarioRepository;
    }

    /// <inheritdoc/>
    public async Task<TratamentoDuracaoResponseDto> PredizirDuracaoTratamentoAsync(TratamentoDuracaoRequestDto requestDto)
    {
      try
      {
        // Verifica se o usuário existe
        var usuario = await _usuarioRepository.GetByIdAsync(requestDto.IdUsuario);
        if (usuario == null)
        {
          throw new ArgumentException($"Usuário com ID {requestDto.IdUsuario} não encontrado");
        }

        // Se o motor de predição ainda não foi inicializado, inicializa-o
        if (_predictionEngine == null)
        {
          if (!await InicializarModeloAsync())
          {
            // Se não foi possível inicializar o modelo, retorna estimativa padrão
            return CriarRespostaEstimativaPadrao(requestDto.TipoTratamento);
          }
        }

        // Cria o objeto de entrada para predição
        var inputData = new TratamentoDuracaoInput
        {
          // Extraímos dados do usuário para enriquecer a predição
          Idade = CalcularIdade(usuario.NrCarteira), // Supondo que temos uma função para estimar idade
          TipoTratamento = requestDto.TipoTratamento,
          ComplexidadeTratamento = requestDto.ComplexidadeTratamento,
          PossuiComorbidades = requestDto.PossuiComorbidades,
          IndiceSaude = requestDto.IndiceSaude,
          TaxaAdesaoAnterior = 0.95f // Valor padrão ou poderia vir de histórico
        };

        // Faz a predição usando o modelo
        var resultado = _predictionEngine!.Predict(inputData);

        // Cria a resposta
        return new TratamentoDuracaoResponseDto
        {
          DuracaoEstimadaSemanas = resultado.DuracaoEstimadaSemanas,
          MensagemEstimativa = CriarMensagemEstimativa(resultado.DuracaoEstimadaSemanas),
          RecomendacoesIniciais = GerarRecomendacoesIniciais(requestDto.TipoTratamento, resultado.DuracaoEstimadaSemanas)
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao prever duração do tratamento");
        return CriarRespostaEstimativaPadrao(requestDto.TipoTratamento);
      }
    }

    /// <inheritdoc/>
    public async Task<bool> TreinarModeloDuracaoAsync()
    {
      try
      {
        _logger.LogInformation("Iniciando treinamento do modelo de duração de tratamento");

        // Dados de treinamento - em produção, viria do banco de dados
        var trainingData = GenerateTratamentoDuracaoSampleData();

        // Carrega os dados para o formato do ML.NET
        var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

        // Define o pipeline de treinamento com a correção para o campo booleano
        var pipeline = _mlContext.Transforms.CopyColumns(
                outputColumnName: "Label",
                inputColumnName: nameof(TratamentoDuracaoInput.ComplexidadeTratamento))
            // Converte o campo booleano para float
            .Append(_mlContext.Transforms.Conversion.ConvertType(
                outputColumnName: "PossuiComorbidadesFloat",
                inputColumnName: nameof(TratamentoDuracaoInput.PossuiComorbidades),
                outputKind: DataKind.Single))
            // Codifica o campo de texto
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                outputColumnName: "TipoTratamentoEncoded",
                inputColumnName: nameof(TratamentoDuracaoInput.TipoTratamento)))
            // Concatena todas as features agora do mesmo tipo
            .Append(_mlContext.Transforms.Concatenate("Features",
                nameof(TratamentoDuracaoInput.Idade),
                "TipoTratamentoEncoded",
                nameof(TratamentoDuracaoInput.ComplexidadeTratamento),
                "PossuiComorbidadesFloat", // Usa a versão convertida
                nameof(TratamentoDuracaoInput.IndiceSaude),
                nameof(TratamentoDuracaoInput.TaxaAdesaoAnterior)))
            .Append(_mlContext.Regression.Trainers.FastTree());

        // Treina o modelo
        var model = pipeline.Fit(trainingDataView);

        // Avalia o modelo (opcional)
        var predictions = model.Transform(trainingDataView);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        _logger.LogInformation($"Modelo treinado com R²: {metrics.RSquared:F2}");

        // Salva o modelo usando o ModelManagerService
        bool salvouModelo = await _modelManagerService.SalvarModeloAsync<TratamentoDuracaoInput, TratamentoDuracaoOutput>(
            MODEL_NAME, model, trainingDataView.Schema);

        if (salvouModelo)
        {
          // Atualiza o motor de predição
          _predictionEngine = _modelManagerService.CreatePredictionEngine<TratamentoDuracaoInput, TratamentoDuracaoOutput>(MODEL_NAME);

          if (_predictionEngine == null)
          {
            _logger.LogWarning("Não foi possível criar o motor de predição após treinamento");
            return false;
          }

          _logger.LogInformation("Motor de predição atualizado com o novo modelo");
          return true;
        }
        else
        {
          _logger.LogWarning("Não foi possível salvar o modelo treinado");
          return false;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao treinar modelo de duração de tratamento");
        return false;
      }
    }

    /// <summary>
    /// Inicializa o modelo de predição
    /// </summary>
    private async Task<bool> InicializarModeloAsync()
    {
      try
      {
        // Verifica se o modelo existe
        if (!await _modelManagerService.VerificarModeloExisteAsync(MODEL_NAME))
        {
          _logger.LogWarning($"Modelo {MODEL_NAME} não encontrado. Tentando criar um novo modelo...");

          // Se o modelo não existe, tenta treiná-lo
          bool treinouModelo = await TreinarModeloDuracaoAsync();
          if (!treinouModelo)
          {
            _logger.LogWarning("Não foi possível treinar um novo modelo. Retornará estimativas padrão.");
            return false;
          }

          return true;
        }

        // Carrega o modelo
        bool modeloCarregado = await _modelManagerService.CarregarModeloAsync<TratamentoDuracaoInput, TratamentoDuracaoOutput>(MODEL_NAME);
        if (!modeloCarregado)
        {
          _logger.LogWarning("Falha ao carregar o modelo. Retornará estimativas padrão.");
          return false;
        }

        // Cria o motor de predição
        _predictionEngine = _modelManagerService.CreatePredictionEngine<TratamentoDuracaoInput, TratamentoDuracaoOutput>(MODEL_NAME);

        if (_predictionEngine == null)
        {
          _logger.LogWarning("Não foi possível criar o motor de predição. Retornará estimativas padrão.");
          return false;
        }

        _logger.LogInformation("Modelo inicializado com sucesso");
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Erro ao inicializar modelo {MODEL_NAME}");
        return false;
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

    /// <summary>
    /// Calcula idade baseada no número da carteira
    /// </summary>
    /// <remarks>
    /// Método ilustrativo - na prática seria necessário acessar a data de nascimento
    /// </remarks>
    private float CalcularIdade(string nrCarteira)
    {
      // Aqui poderíamos extrair a idade real do usuário,
      // mas para simplificar vamos retornar um valor médio
      return 35.0f;
    }

    /// <summary>
    /// Cria uma mensagem explicativa com base na duração estimada
    /// </summary>
    private string CriarMensagemEstimativa(float duracaoSemanas)
    {
      int meses = (int)(duracaoSemanas / 4);

      if (meses < 3)
        return $"Tratamento de curta duração (aproximadamente {meses} meses)";
      else if (meses < 6)
        return $"Tratamento de média duração (aproximadamente {meses} meses)";
      else
        return $"Tratamento de longa duração (aproximadamente {meses} meses)";
    }

    /// <summary>
    /// Gera recomendações iniciais com base no tipo e duração do tratamento
    /// </summary>
    private List<string> GerarRecomendacoesIniciais(string tipoTratamento, float duracaoSemanas)
    {
      var recomendacoes = new List<string>();

      // Recomendações comuns a todos os tratamentos
      recomendacoes.Add("Manter boa higiene bucal com escovação após as refeições");
      recomendacoes.Add("Seguir rigorosamente as orientações do dentista");

      // Recomendações específicas por tipo de tratamento
      switch (tipoTratamento.ToLower())
      {
        case "ortodontia":
          recomendacoes.Add("Evitar alimentos duros ou pegajosos que possam danificar o aparelho");
          recomendacoes.Add("Utilizar escovas específicas para aparelhos ortodônticos");
          break;
        case "implante":
          recomendacoes.Add("Evitar fumar durante o processo de cicatrização");
          recomendacoes.Add("Seguir rigorosamente as medicações prescritas");
          break;
        case "canal":
          recomendacoes.Add("Evitar mastigar com o dente tratado até a restauração definitiva");
          recomendacoes.Add("Seguir as recomendações de medicação para controle da dor");
          break;
        default:
          recomendacoes.Add("Comparecer a todas as consultas agendadas");
          break;
      }

      // Recomendações baseadas na duração
      if (duracaoSemanas > 12)
      {
        recomendacoes.Add("Prepare-se para um tratamento de longo prazo com múltiplas visitas");
        recomendacoes.Add("Considere agendar consultas com antecedência para garantir disponibilidade");
      }

      return recomendacoes;
    }

    /// <summary>
    /// Cria uma resposta com estimativa padrão quando o modelo não está disponível
    /// </summary>
    private TratamentoDuracaoResponseDto CriarRespostaEstimativaPadrao(string tipoTratamento)
    {
      // Valores padrão por tipo de tratamento (em semanas)
      float duracaoPadrao = tipoTratamento.ToLower() switch
      {
        "ortodontia" => 104, // 2 anos
        "implante" => 16,    // 4 meses
        "canal" => 3,        // 3 semanas
        "limpeza" => 1,      // 1 semana
        "clareamento" => 4,  // 1 mês
        _ => 12              // 3 meses (padrão)
      };

      return new TratamentoDuracaoResponseDto
      {
        DuracaoEstimadaSemanas = duracaoPadrao,
        MensagemEstimativa = CriarMensagemEstimativa(duracaoPadrao),
        RecomendacoesIniciais = GerarRecomendacoesIniciais(tipoTratamento, duracaoPadrao)
      };
    }
  }
}