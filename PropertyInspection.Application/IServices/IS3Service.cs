using System.IO;

namespace PropertyInspection.Application.IServices
{
    public interface IS3Service
    {
        Task<string> GeneratePresignedUploadUrlAsync(string folder, string fileName, string contentType);
        Task<string> GeneratePresignedDownloadUrlAsync(string folder, string fileName);
        Task UploadFileAsync(Stream fileStream, string folder, string fileName, string contentType);
        Task DeleteFileAsync(string folder, string fileName);
        Task<bool> FileExistsAsync(string folder, string fileName);
    }
}
