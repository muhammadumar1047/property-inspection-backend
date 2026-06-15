using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class SignatureService : ISignatureService
    {
        private readonly IS3Service _s3Service;
        private readonly IUnitOfWork _unitOfWork;

        public SignatureService(
            IS3Service s3Service,
            IUnitOfWork unitOfWork)
        {
            _s3Service = s3Service;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<SignatureUploadUrlResponse>> GenerateUploadUrlAsync(
            Guid userId,
            GenerateSignatureUploadUrlRequest request)
        {
            try
            {
                var folder = $"signatures/{userId}";
                var result = await _s3Service.GeneratePresignedUploadUrlWithKeyAsync(
                    folder,
                    request.FileName,
                    request.ContentType);

                return result;
            }
            catch
            {
                return new ServiceResponse<SignatureUploadUrlResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> SaveSignatureAsync(
            Guid userId,
            SaveSignatureRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.FileKey))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                // Verify the file exists in S3
                var folder = $"signatures/{userId}";
                var fileName = request.FileKey.Replace($"{folder}/", string.Empty);
                var existsResult = await _s3Service.FileExistsAsync(folder, fileName);
                if (!existsResult.Success || !existsResult.Data)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Signature file not found in storage. Please upload the file first.",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var fileUrl = _s3Service.BuildFileUrl(request.FileKey);

                user.SignatureImage = fileUrl;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Signature saved successfully",
                    Data = true
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
    }
}