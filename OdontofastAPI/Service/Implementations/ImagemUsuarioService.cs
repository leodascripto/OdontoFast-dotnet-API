// OdontofastAPI/Service/Implementations/ImagemUsuarioService.cs
using OdontofastAPI.DTO;
using OdontofastAPI.Model;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations
{
  public class ImagemUsuarioService : IImagemUsuarioService
  {
    private readonly IImagemUsuarioRepository _imagemUsuarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ImagemUsuarioService(
        IImagemUsuarioRepository imagemUsuarioRepository,
        IUsuarioRepository usuarioRepository)
    {
      _imagemUsuarioRepository = imagemUsuarioRepository;
      _usuarioRepository = usuarioRepository;
    }

    public async Task<ImagemUsuarioDTO?> GetImagemUsuarioByIdAsync(long idUsuario)
    {
      var imagemUsuario = await _imagemUsuarioRepository.GetByIdAsync(idUsuario);
      if (imagemUsuario == null)
      {
        return null;
      }

      return new ImagemUsuarioDTO
      {
        IdUsuario = imagemUsuario.IdUsuario,
        CaminhoImg = imagemUsuario.CaminhoImg
      };
    }

    public async Task<ImagemUsuarioDTO> CreateImagemUsuarioAsync(ImagemUsuarioCreateDTO imagemUsuarioCreateDto)
    {
      // Verifica se o usuário existe
      var usuarioExiste = await _usuarioRepository.GetByIdAsync(imagemUsuarioCreateDto.IdUsuario);
      if (usuarioExiste == null)
      {
        throw new ArgumentException($"Usuário com ID {imagemUsuarioCreateDto.IdUsuario} não encontrado.");
      }

      // Verifica se já existe uma imagem para este usuário
      var imagemExistente = await _imagemUsuarioRepository.ExistsAsync(imagemUsuarioCreateDto.IdUsuario);
      if (imagemExistente)
      {
        throw new InvalidOperationException($"Já existe uma imagem para o usuário com ID {imagemUsuarioCreateDto.IdUsuario}. Use o método de atualização.");
      }

      var imagemUsuario = new ImagemUsuario
      {
        IdUsuario = imagemUsuarioCreateDto.IdUsuario,
        CaminhoImg = imagemUsuarioCreateDto.CaminhoImg
      };

      var imagemCriada = await _imagemUsuarioRepository.CreateAsync(imagemUsuario);

      return new ImagemUsuarioDTO
      {
        IdUsuario = imagemCriada.IdUsuario,
        CaminhoImg = imagemCriada.CaminhoImg
      };
    }

    public async Task<ImagemUsuarioDTO?> UpdateImagemUsuarioAsync(long idUsuario, ImagemUsuarioUpdateDTO imagemUsuarioUpdateDto)
    {
      // Verifica se o usuário existe
      var usuarioExiste = await _usuarioRepository.GetByIdAsync(idUsuario);
      if (usuarioExiste == null)
      {
        throw new ArgumentException($"Usuário com ID {idUsuario} não encontrado.");
      }

      var imagemUsuario = new ImagemUsuario
      {
        IdUsuario = idUsuario,
        CaminhoImg = imagemUsuarioUpdateDto.CaminhoImg
      };

      var imagemAtualizada = await _imagemUsuarioRepository.UpdateAsync(imagemUsuario);
      if (imagemAtualizada == null)
      {
        return null;
      }

      return new ImagemUsuarioDTO
      {
        IdUsuario = imagemAtualizada.IdUsuario,
        CaminhoImg = imagemAtualizada.CaminhoImg
      };
    }

    public async Task<bool> DeleteImagemUsuarioAsync(long idUsuario)
    {
      // Verifica se o usuário existe
      var usuarioExiste = await _usuarioRepository.GetByIdAsync(idUsuario);
      if (usuarioExiste == null)
      {
        throw new ArgumentException($"Usuário com ID {idUsuario} não encontrado.");
      }

      return await _imagemUsuarioRepository.DeleteAsync(idUsuario);
    }

    public async Task<bool> ExistsImagemUsuarioAsync(long idUsuario)
    {
      return await _imagemUsuarioRepository.ExistsAsync(idUsuario);
    }
  }
}