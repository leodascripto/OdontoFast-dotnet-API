using Microsoft.EntityFrameworkCore;
using OdontofastAPI.Data;
using OdontofastAPI.Model;
using OdontofastAPI.Repository.Interfaces;
using System.Threading.Tasks;

namespace OdontofastAPI.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly OdontofastDbContext _context;

        public UsuarioRepository(OdontofastDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuarioByCarteira(string nrCarteira)
        {
            return await _context.Usuarios
                .Where(u => u.NrCarteira == nrCarteira)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> UpdateAsync(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            if (existingUsuario == null)
            {
                return null;
            }

            existingUsuario.NomeUsuario = usuario.NomeUsuario;
            existingUsuario.SenhaUsuario = usuario.SenhaUsuario;
            existingUsuario.EmailUsuario = usuario.EmailUsuario;
            existingUsuario.NrCarteira = usuario.NrCarteira;
            existingUsuario.TelefoneUsuario = usuario.TelefoneUsuario;

            _context.Usuarios.Update(existingUsuario);
            await _context.SaveChangesAsync();

            return existingUsuario;
        }
    }
}