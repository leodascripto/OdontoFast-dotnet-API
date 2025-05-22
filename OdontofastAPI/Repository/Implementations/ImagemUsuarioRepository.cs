// OdontofastAPI/Repository/Implementations/ImagemUsuarioRepository.cs
using Microsoft.EntityFrameworkCore;
using OdontofastAPI.Data;
using OdontofastAPI.Model;
using OdontofastAPI.Repository.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Repository.Implementations
{
  public class ImagemUsuarioRepository : IImagemUsuarioRepository
  {
    private readonly OdontofastDbContext _context;

    public ImagemUsuarioRepository(OdontofastDbContext context)
    {
      _context = context;
    }

    public async Task<ImagemUsuario?> GetByIdAsync(long idUsuario)
    {
      return await _context.ImagensUsuario
          .Include(img => img.Usuario)
          .FirstOrDefaultAsync(img => img.IdUsuario == idUsuario);
    }

    public async Task<ImagemUsuario> CreateAsync(ImagemUsuario imagemUsuario)
    {
      _context.ImagensUsuario.Add(imagemUsuario);
      await _context.SaveChangesAsync();
      return imagemUsuario;
    }

    public async Task<ImagemUsuario?> UpdateAsync(ImagemUsuario imagemUsuario)
    {
      var existingImagem = await _context.ImagensUsuario.FindAsync(imagemUsuario.IdUsuario);
      if (existingImagem == null)
      {
        return null;
      }

      existingImagem.CaminhoImg = imagemUsuario.CaminhoImg;

      _context.ImagensUsuario.Update(existingImagem);
      await _context.SaveChangesAsync();

      return existingImagem;
    }

    public async Task<bool> DeleteAsync(long idUsuario)
    {
      var imagemUsuario = await _context.ImagensUsuario.FindAsync(idUsuario);
      if (imagemUsuario == null)
      {
        return false;
      }

      _context.ImagensUsuario.Remove(imagemUsuario);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> ExistsAsync(long idUsuario)
    {
      // Solução alternativa para Oracle: usar Count ao invés de Any
      var count = await _context.ImagensUsuario
          .Where(img => img.IdUsuario == idUsuario)
          .CountAsync();

      return count > 0;
    }

    // Método alternativo caso ainda haja problemas
    public async Task<bool> ExistsAsyncAlternative(long idUsuario)
    {
      // Outra abordagem: buscar o primeiro registro e verificar se não é null
      var imagem = await _context.ImagensUsuario
          .Where(img => img.IdUsuario == idUsuario)
          .FirstOrDefaultAsync();

      return imagem != null;
    }
  }
}