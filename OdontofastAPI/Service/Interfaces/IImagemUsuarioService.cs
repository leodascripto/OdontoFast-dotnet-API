// OdontofastAPI/Service/Interfaces/IImagemUsuarioService.cs
using OdontofastAPI.DTO;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces
{
  public interface IImagemUsuarioService
  {
    Task<ImagemUsuarioDTO?> GetImagemUsuarioByIdAsync(long idUsuario);
    Task<ImagemUsuarioDTO> CreateImagemUsuarioAsync(ImagemUsuarioCreateDTO imagemUsuarioCreateDto);
    Task<ImagemUsuarioDTO?> UpdateImagemUsuarioAsync(long idUsuario, ImagemUsuarioUpdateDTO imagemUsuarioUpdateDto);
    Task<bool> DeleteImagemUsuarioAsync(long idUsuario);
    Task<bool> ExistsImagemUsuarioAsync(long idUsuario);
  }
}