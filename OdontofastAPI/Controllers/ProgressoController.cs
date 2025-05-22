using Microsoft.AspNetCore.Mvc;
using OdontofastAPI.DTO;
using OdontofastAPI.Service.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressoController : ControllerBase
    {
        private readonly IProgressoService _progressoService;

        public ProgressoController(IProgressoService progressoService)
        {
            _progressoService = progressoService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProgressoDTO progressoDto)
        {
            try
            {
                await _progressoService.ProcessarProgressoAsync(progressoDto);
                return Ok(new { message = "Progresso recebido e processado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", error = ex.Message });
            }
        }
    }
}
