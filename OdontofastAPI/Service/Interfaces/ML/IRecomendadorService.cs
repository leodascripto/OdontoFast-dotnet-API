using OdontofastAPI.DTO.ML;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces.ML
{
    /// <summary>
    /// Interface para o serviço de recomendações personalizadas
    /// </summary>
    public interface IRecomendadorService
    {
        /// <summary>
        /// Gera recomendações personalizadas com base no progresso e características do paciente
        /// </summary>
        /// <param name="requestDto">Dados do paciente e progresso do tratamento</param>
        /// <returns>Recomendações personalizadas e próximas etapas</returns>
        Task<RecomendacaoResponseDto> GerarRecomendacoesPersonalizadasAsync(RecomendacaoRequestDto requestDto);
    }
}