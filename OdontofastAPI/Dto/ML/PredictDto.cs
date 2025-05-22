namespace OdontofastAPI.DTO.ML
{
  /// <summary>
  /// DTO para solicitação de predição de duração de tratamento
  /// </summary>
  public class TratamentoDuracaoRequestDto
  {
    public required long IdUsuario { get; set; }
    public required string TipoTratamento { get; set; }
    public float ComplexidadeTratamento { get; set; }
    public bool PossuiComorbidades { get; set; }
    public float IndiceSaude { get; set; }
  }

  /// <summary>
  /// DTO para resposta de predição de duração de tratamento
  /// </summary>
  public class TratamentoDuracaoResponseDto
  {
    public float DuracaoEstimadaSemanas { get; set; }
    public string MensagemEstimativa { get; set; } = string.Empty;
    public List<string> RecomendacoesIniciais { get; set; } = new List<string>();
  }

  /// <summary>
  /// DTO para solicitação de recomendações personalizadas
  /// </summary>
  public class RecomendacaoRequestDto
  {
    public required long IdUsuario { get; set; }
    public float ProgressoAtual { get; set; }
    public required string TipoTratamento { get; set; }
  }

  /// <summary>
  /// DTO para resposta de recomendações personalizadas
  /// </summary>
  public class RecomendacaoResponseDto
  {
    public List<string> RecomendacoesCuidados { get; set; } = new List<string>();
    public List<string> RecomendacoesProximasEtapas { get; set; } = new List<string>();
    public string MensagemPersonalizada { get; set; } = string.Empty;
  }
}