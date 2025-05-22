// Services/interfaces/IEmailService.cs
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailMotivacionalAsync(string destinatario, string mensagem);
    }
}
