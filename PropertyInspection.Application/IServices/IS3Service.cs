using System.IO;

namespace PropertyInspection.Application.IServices
{
    public interface IS3Service
    {
        Task<PropertyInspection.Shared.ServiceResponse<string>> GeneratePresignedUploadUrlAsync(string folder, string fileName, string contentType);
        Task<PropertyInspection.Shared.ServiceResponse<string>> GeneratePresignedDownloadUrlAsync(string folder, string fileName);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> UploadFileAsync(Stream fileStream, string folder, string fileName, string contentType);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> DeleteFileAsync(string folder, string fileName);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> FileExistsAsync(string folder, string fileName);
    }
}
