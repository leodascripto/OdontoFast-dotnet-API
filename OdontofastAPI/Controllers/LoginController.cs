using Microsoft.AspNetCore.Mvc;
using OdontofastAPI.Model;
using OdontofastAPI.Service.Interfaces;

namespace OdontofastAPI.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public LoginController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Realiza o login com as credenciais fornecidas
                var usuario = await _usuarioService.Login(loginDto.NrCarteira, loginDto.Senha);

                if (usuario == null)
                {
                    return Unauthorized(new { message = "Credenciais inválidas." });
                }

                // Retorna os dados do usuário após login bem-sucedido
                return Ok(new
                {
                    usuario.IdUsuario,
                    usuario.NomeUsuario,
                    usuario.EmailUsuario,
                    usuario.NrCarteira,
                    usuario.TelefoneUsuario
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", error = ex.Message });
            }
        }
    }
}
