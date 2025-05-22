using Microsoft.AspNetCore.Mvc;
using OdontofastAPI.DTO;
using OdontofastAPI.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace OdontofastAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ImagemUsuarioController : ControllerBase
  {
    private readonly IImagemUsuarioService _imagemUsuarioService;
    private readonly ILogger<ImagemUsuarioController> _logger;

    public ImagemUsuarioController(
        IImagemUsuarioService imagemUsuarioService,
        ILogger<ImagemUsuarioController> logger)
    {
      _imagemUsuarioService = imagemUsuarioService;
      _logger = logger;
    }

    /// <summary>
    /// Obtém a imagem de perfil de um usuário pelo ID
    /// </summary>
    /// <param name="idUsuario">ID do usuário</param>
    /// <returns>Dados da imagem do usuário</returns>
    [HttpGet("{idUsuario}")]
    public async Task<IActionResult> GetImagemUsuario(long idUsuario)
    {
      try
      {
        var imagemUsuario = await _imagemUsuarioService.GetImagemUsuarioByIdAsync(idUsuario);
        if (imagemUsuario == null)
        {
          return NotFound(new { Message = "Imagem de usuário não encontrada." });
        }

        return Ok(imagemUsuario);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao buscar imagem do usuário {IdUsuario}", idUsuario);
        return StatusCode(500, new { Message = "Erro interno no servidor.", Error = ex.Message });
      }
    }

    /// <summary>
    /// Cria uma nova imagem de perfil para um usuário
    /// </summary>
    /// <param name="imagemUsuarioCreateDto">Dados da imagem a ser criada</param>
    /// <returns>Dados da imagem criada</returns>
    [HttpPost]
    public async Task<IActionResult> CreateImagemUsuario([FromBody] ImagemUsuarioCreateDTO imagemUsuarioCreateDto)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        var imagemCriada = await _imagemUsuarioService.CreateImagemUsuarioAsync(imagemUsuarioCreateDto);
        return CreatedAtAction(
            nameof(GetImagemUsuario),
            new { idUsuario = imagemCriada.IdUsuario },
            imagemCriada);
      }
      catch (ArgumentException ex)
      {
        _logger.LogWarning(ex, "Tentativa de criar imagem para usuário inexistente: {IdUsuario}", imagemUsuarioCreateDto.IdUsuario);
        return BadRequest(new { Message = ex.Message });
      }
      catch (InvalidOperationException ex)
      {
        _logger.LogWarning(ex, "Tentativa de criar imagem duplicada para usuário: {IdUsuario}", imagemUsuarioCreateDto.IdUsuario);
        return Conflict(new { Message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao criar imagem do usuário {IdUsuario}", imagemUsuarioCreateDto.IdUsuario);
        return StatusCode(500, new { Message = "Erro interno no servidor.", Error = ex.Message });
      }
    }

    /// <summary>
    /// Atualiza a imagem de perfil de um usuário
    /// </summary>
    /// <param name="idUsuario">ID do usuário</param>
    /// <param name="imagemUsuarioUpdateDto">Novos dados da imagem</param>
    /// <returns>Dados da imagem atualizada</returns>
    [HttpPut("{idUsuario}")]
    public async Task<IActionResult> UpdateImagemUsuario(long idUsuario, [FromBody] ImagemUsuarioUpdateDTO imagemUsuarioUpdateDto)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        var imagemAtualizada = await _imagemUsuarioService.UpdateImagemUsuarioAsync(idUsuario, imagemUsuarioUpdateDto);
        if (imagemAtualizada == null)
        {
          return NotFound(new { Message = "Imagem de usuário não encontrada." });
        }

        return Ok(imagemAtualizada);
      }
      catch (ArgumentException ex)
      {
        _logger.LogWarning(ex, "Tentativa de atualizar imagem para usuário inexistente: {IdUsuario}", idUsuario);
        return BadRequest(new { Message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao atualizar imagem do usuário {IdUsuario}", idUsuario);
        return StatusCode(500, new { Message = "Erro interno no servidor.", Error = ex.Message });
      }
    }

    /// <summary>
    /// Remove a imagem de perfil de um usuário
    /// </summary>
    /// <param name="idUsuario">ID do usuário</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{idUsuario}")]
    public async Task<IActionResult> DeleteImagemUsuario(long idUsuario)
    {
      try
      {
        var deletado = await _imagemUsuarioService.DeleteImagemUsuarioAsync(idUsuario);
        if (!deletado)
        {
          return NotFound(new { Message = "Imagem de usuário não encontrada." });
        }

        return Ok(new { Message = "Imagem de usuário excluída com sucesso." });
      }
      catch (ArgumentException ex)
      {
        _logger.LogWarning(ex, "Tentativa de excluir imagem para usuário inexistente: {IdUsuario}", idUsuario);
        return BadRequest(new { Message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao excluir imagem do usuário {IdUsuario}", idUsuario);
        return StatusCode(500, new { Message = "Erro interno no servidor.", Error = ex.Message });
      }
    }

    /// <summary>
    /// Verifica se um usuário possui imagem de perfil
    /// </summary>
    /// <param name="idUsuario">ID do usuário</param>
    /// <returns>True se o usuário possui imagem, False caso contrário</returns>
    [HttpGet("{idUsuario}/exists")]
    public async Task<IActionResult> CheckImagemUsuarioExists(long idUsuario)
    {
      try
      {
        var exists = await _imagemUsuarioService.ExistsImagemUsuarioAsync(idUsuario);
        return Ok(new { IdUsuario = idUsuario, PossuiImagem = exists });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao verificar existência de imagem do usuário {IdUsuario}", idUsuario);
        return StatusCode(500, new { Message = "Erro interno no servidor.", Error = ex.Message });
      }
    }
  }
}