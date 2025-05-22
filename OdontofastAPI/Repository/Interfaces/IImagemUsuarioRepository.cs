using OdontofastAPI.Model;
using System.Threading.Tasks;

namespace OdontofastAPI.Repository.Interfaces
{
  public interface IImagemUsuarioRepository
  {
    Task<ImagemUsuario?> GetByIdAsync(long idUsuario);
    Task<ImagemUsuario> CreateAsync(ImagemUsuario imagemUsuario);
    Task<ImagemUsuario?> UpdateAsync(ImagemUsuario imagemUsuario);
    Task<bool> DeleteAsync(long idUsuario);
    Task<bool> ExistsAsync(long idUsuario);
  }
}