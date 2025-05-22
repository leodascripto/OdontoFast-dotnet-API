using OdontofastAPI.DTO.ML;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces.ML
{
  /// <summary>
  /// Interface para o serviço de predição de duração e características de tratamentos
  /// </summary>
  public interface IPreditorTratamentoService
  {
    /// <summary>
    /// Prediz a duração estimada do tratamento baseado nas características do paciente e tipo de tratamento
    /// </summary>
    /// <param name="requestDto">Dados do paciente e tratamento</param>
    /// <returns>Estimativa de duração e recomendações iniciais</returns>
    Task<TratamentoDuracaoResponseDto> PredizirDuracaoTratamentoAsync(TratamentoDuracaoRequestDto requestDto);

    /// <summary>
    /// Treina novamente o modelo de predição de tratamento com dados atualizados
    /// </summary>
    /// <returns>Métricas de desempenho do modelo</returns>
    Task<bool> TreinarModeloDuracaoAsync();
  }
}