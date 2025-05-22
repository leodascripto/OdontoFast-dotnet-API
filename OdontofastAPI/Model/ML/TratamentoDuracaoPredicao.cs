using Microsoft.ML.Data;

namespace OdontofastAPI.Model.ML
{
  /// <summary>
  /// Modelo de entrada para predição de duração de tratamentos
  /// </summary>
  public class TratamentoDuracaoInput
  {
    [LoadColumn(0)]
    public float Idade { get; set; }

    [LoadColumn(1)]
    public string TipoTratamento { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float ComplexidadeTratamento { get; set; }

    [LoadColumn(3)]
    public bool PossuiComorbidades { get; set; }

    [LoadColumn(4)]
    public float IndiceSaude { get; set; }

    [LoadColumn(5)]
    public float TaxaAdesaoAnterior { get; set; }
  }

  /// <summary>
  /// Modelo de saída para predição de duração de tratamentos (semanas)
  /// </summary>
  public class TratamentoDuracaoOutput
  {
    [ColumnName("Score")]
    public float DuracaoEstimadaSemanas { get; set; }
  }

  /// <summary>
  /// Modelo para recomendações personalizadas
  /// </summary>
  public class RecomendacaoInput
  {
    [LoadColumn(0)]
    public string TipoTratamento { get; set; } = string.Empty;

    [LoadColumn(1)]
    public float ProgressoAtual { get; set; }

    [LoadColumn(2)]
    public float Idade { get; set; }

    [LoadColumn(3)]
    public string HistoricoTratamento { get; set; } = string.Empty;

    [LoadColumn(4)]
    public string CondicoesMedicas { get; set; } = string.Empty;
  }

  /// <summary>
  /// Categoria de recomendação
  /// </summary>
  public class RecomendacaoOutput
  {
    [ColumnName("PredictedLabel")]
    public string CategoriaRecomendacao { get; set; } = string.Empty;
  }
}