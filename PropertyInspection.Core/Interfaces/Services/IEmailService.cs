using System.Threading.Tasks;

namespace PropertyInspection.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, string? fromEmail = null, string? fromName = null);
    }
}
