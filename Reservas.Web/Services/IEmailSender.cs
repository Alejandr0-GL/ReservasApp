using System.Threading.Tasks;

namespace Reservas.Web.Services
{
    public interface IEmailSender
    {
        Task EnviarAsync(string destinatario, string asunto, string html);
    }
}