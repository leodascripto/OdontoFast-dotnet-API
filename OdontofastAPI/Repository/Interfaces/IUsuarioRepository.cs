using OdontofastAPI.Model;
using System.Threading.Tasks;

namespace OdontofastAPI.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioByCarteira(string nrCarteira);
        Task<Usuario?> GetByIdAsync(long id);
        Task<Usuario?> UpdateAsync(Usuario usuario);
    }
}