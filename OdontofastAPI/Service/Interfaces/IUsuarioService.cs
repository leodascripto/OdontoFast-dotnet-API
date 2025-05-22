using OdontofastAPI.DTO;
using OdontofastAPI.Model;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> Login(string nrCarteira, string senha);
        Task<UsuarioDTO?> GetUsuarioByIdAsync(long id);
        Task<UsuarioDTO?> UpdateUsuarioAsync(long id, UsuarioDTO usuarioDTO);
    }
}