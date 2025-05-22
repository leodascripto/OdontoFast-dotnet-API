using OdontofastAPI.DTO;
using OdontofastAPI.Model;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> Login(string nrCarteira, string senha)
        {
            var usuario = await _usuarioRepository.GetUsuarioByCarteira(nrCarteira);
            if (usuario == null || usuario.SenhaUsuario != senha)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }
            return usuario;
        }

        public async Task<UsuarioDTO?> GetUsuarioByIdAsync(long id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return null;
            }

            return new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                SenhaUsuario = usuario.SenhaUsuario, // Adicionado para satisfazer a propriedade required
                EmailUsuario = usuario.EmailUsuario,
                NrCarteira = usuario.NrCarteira,
                TelefoneUsuario = usuario.TelefoneUsuario
            };
        }

        public async Task<UsuarioDTO?> UpdateUsuarioAsync(long id, UsuarioDTO usuarioDTO)
        {
            // Busca o usuário existente pelo ID
            var usuarioExistente = await _usuarioRepository.GetByIdAsync(id);

            // Se o usuário não existe, retorna null
            if (usuarioExistente == null)
            {
                return null;
            }

            // Atualiza apenas os campos do usuário existente com os dados do DTO
            usuarioExistente.NomeUsuario = usuarioDTO.NomeUsuario ?? usuarioExistente.NomeUsuario;
            usuarioExistente.SenhaUsuario = usuarioDTO.SenhaUsuario ?? usuarioExistente.SenhaUsuario;
            usuarioExistente.EmailUsuario = usuarioDTO.EmailUsuario ?? usuarioExistente.EmailUsuario;
            usuarioExistente.NrCarteira = usuarioDTO.NrCarteira ?? usuarioExistente.NrCarteira;
            usuarioExistente.TelefoneUsuario = usuarioDTO.TelefoneUsuario != 0 ? usuarioDTO.TelefoneUsuario : usuarioExistente.TelefoneUsuario;

            // Chama o repositório para atualizar o usuário no banco de dados
            var updatedUsuario = await _usuarioRepository.UpdateAsync(usuarioExistente);

            // Verifica se a atualização retornou um usuário válido
            if (updatedUsuario == null)
            {
                return null;
            }

            // Retorna o DTO com os dados atualizados
            return new UsuarioDTO
            {
                IdUsuario = updatedUsuario.IdUsuario,
                NomeUsuario = updatedUsuario.NomeUsuario,
                SenhaUsuario = updatedUsuario.SenhaUsuario, // Adicionado para satisfazer a propriedade required
                EmailUsuario = updatedUsuario.EmailUsuario,
                NrCarteira = updatedUsuario.NrCarteira,
                TelefoneUsuario = updatedUsuario.TelefoneUsuario
            };
        }
    }
}