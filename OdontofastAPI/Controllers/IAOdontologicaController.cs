// OdontofastAPI/Controllers/IAOdontologicaController.cs
using Microsoft.AspNetCore.Mvc;
using OdontofastAPI.DTO.ML;
using OdontofastAPI.Service.Interfaces.ML;
using System;
using System.Threading.Tasks;

namespace OdontofastAPI.Controllers
{
  /// <summary>
  /// Controlador para acesso às funcionalidades de IA do sistema
  /// </summary>
  [ApiController]
  [Route("api/[controller]")]
  public class IAOdontologicaController : ControllerBase
  {
    private readonly IPreditorTratamentoService _preditorTratamentoService;
    private readonly IRecomendadorService _recomendadorService;
    private readonly ILogger<IAOdontologicaController> _logger;

    public IAOdontologicaController(
        IPreditorTratamentoService preditorTratamentoService,
        IRecomendadorService recomendadorService,
        ILogger<IAOdontologicaController> logger)
    {
      _preditorTratamentoService = preditorTratamentoService;
      _recomendadorService = recomendadorService;
      _logger = logger;
    }

    /// <summary>
    /// Prediz a duração estimada do tratamento com base nos dados do paciente
    /// </summary>
    /// <param name="requestDto">Dados do paciente e tratamento</param>
    /// <returns>Estimativa de duração e recomendações</returns>
    [HttpPost("prever-tratamento")]
    public async Task<IActionResult> PreverDuracaoTratamento([FromBody] TratamentoDuracaoRequestDto requestDto)
    {
      try
      {
        _logger.LogInformation($"Solicitação de previsão de duração de tratamento para usuário {requestDto.IdUsuario}");
        var resultado = await _preditorTratamentoService.PredizirDuracaoTratamentoAsync(requestDto);
        return Ok(resultado);
      }
      catch (ArgumentException ex)
      {
        _logger.LogWarning(ex, "Erro de validação ao prever duração do tratamento");
        return BadRequest(new { mensagem = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao prever duração do tratamento");
        return StatusCode(500, new { mensagem = "Erro interno ao processar a solicitação.", erro = ex.Message });
      }
    }


    /// <summary>
    /// Gera recomendações personalizadas para o paciente
    /// </summary>
    /// <param name="requestDto">Dados do paciente e progresso do tratamento</param>
    /// <returns>Recomendações personalizadas</returns>
    [HttpPost("recomendar")]
    public async Task<IActionResult> GerarRecomendacoes([FromBody] RecomendacaoRequestDto requestDto)
    {
      try
      {
        _logger.LogInformation($"Solicitação de recomendações para usuário {requestDto.IdUsuario}");
        var resultado = await _recomendadorService.GerarRecomendacoesPersonalizadasAsync(requestDto);
        return Ok(resultado);
      }
      catch (ArgumentException ex)
      {
        _logger.LogWarning(ex, "Erro de validação ao gerar recomendações");
        return BadRequest(new { mensagem = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao gerar recomendações personalizadas");
        return StatusCode(500, new { mensagem = "Erro interno ao processar a solicitação.", erro = ex.Message });
      }
    }

    /// <summary>
    /// Solicita treinamento ou atualização do modelo de predição de tratamento
    /// </summary>
    /// <remarks>
    /// Esta função é administrativa e deveria ser protegida por autenticação em produção
    /// </remarks>
    [HttpPost("treinar-modelo-duracao")]
    public async Task<IActionResult> TreinarModeloDuracao()
    {
      try
      {
        _logger.LogInformation("Solicitação de treinamento do modelo de duração de tratamento");
        var sucesso = await _preditorTratamentoService.TreinarModeloDuracaoAsync();

        if (sucesso)
        {
          return Ok(new { mensagem = "Modelo treinado com sucesso." });
        }
        else
        {
          return StatusCode(500, new { mensagem = "Não foi possível treinar o modelo." });
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao treinar modelo de duração de tratamento");
        return StatusCode(500, new { mensagem = "Erro interno ao treinar o modelo.", erro = ex.Message });
      }
    }

  }
}