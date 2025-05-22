using OdontofastAPI.DTO;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations
{
    public class ProgressoService : IProgressoService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService;

        public ProgressoService(IUsuarioRepository usuarioRepository, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        } 

        public async Task ProcessarProgressoAsync(ProgressoDTO progressoDto)
        {
            if (progressoDto.Progresso >= 1.0)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(progressoDto.IdUsuario);
                if (usuario != null)
                {
                    var mensagem = "Parabéns! Você completou todos os cuidados hoje.";
                    await _emailService.EnviarEmailMotivacionalAsync(usuario.EmailUsuario, mensagem);
                }
            }
        }
    }
}
