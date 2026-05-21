using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IS3StorageService
    {
        Task<string> UploadLogoAsync(IFormFile file, string agencyId);
    }
}
