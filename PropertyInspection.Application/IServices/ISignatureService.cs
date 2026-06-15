using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface ISignatureService
    {
        Task<ServiceResponse<SignatureUploadUrlResponse>> GenerateUploadUrlAsync(Guid userId, GenerateSignatureUploadUrlRequest request);
        Task<ServiceResponse<bool>> SaveSignatureAsync(Guid userId, SaveSignatureRequest request);
    }
}