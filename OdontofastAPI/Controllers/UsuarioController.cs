using Microsoft.AspNetCore.Mvc;
using OdontofastAPI.DTO;
using OdontofastAPI.Service.Interfaces;

namespace OdontofastAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET /api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(long id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }
            return Ok(usuario);
        }

        // PUT /api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(long id, UsuarioDTO usuarioDTO)
        {
            var updatedUsuario = await _usuarioService.UpdateUsuarioAsync(id, usuarioDTO);
            if (updatedUsuario == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }
            return Ok(updatedUsuario);
        }
  
    }
}
