using OdontofastAPI.DTO;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces
{
    public interface IProgressoService
    {
        Task ProcessarProgressoAsync(ProgressoDTO progressoDto);
    }
}
