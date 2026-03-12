using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class S3Service : IS3Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IOptions<AwsSettings> awsSettings, IAmazonS3 s3Client , IUnitOfWork unitOfWork)
        {
            _s3Client = s3Client;
            _bucketName = awsSettings.Value.S3Bucket;
            _unitOfWork = unitOfWork;
        }

        public Task<ServiceResponse<string>> GeneratePresignedUploadUrlAsync(string folder, string fileName, string contentType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
                {
                    return Task.FromResult(new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    });
                }

                var key = $"{folder}/{Guid.NewGuid()}_{fileName}";

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    ContentType = contentType
                };

                var url = _s3Client.GetPreSignedURL(request);
                return Task.FromResult(new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = url
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }

        public Task<ServiceResponse<string>> GeneratePresignedDownloadUrlAsync(string folder, string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
                {
                    return Task.FromResult(new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    });
                }

                var key = $"{folder}/{fileName}";

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddMinutes(5)
                };

                var url = _s3Client.GetPreSignedURL(request);
                return Task.FromResult(new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = url
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }

        public async Task<ServiceResponse<bool>> UploadFileAsync(Stream fileStream, string folder, string fileName, string contentType)
        {
            try
            {
                if (fileStream == null || string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var key = $"{folder}/{fileName}";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fileStream,
                    Key = key,
                    BucketName = _bucketName,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.Private
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "File uploaded successfully",
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

        public async Task<ServiceResponse<bool>> DeleteFileAsync(string folder, string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var key = $"{folder}/{fileName}";

                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "File deleted successfully",
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

        public async Task<ServiceResponse<bool>> FileExistsAsync(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                };
            }

            var key = $"{folder}/{fileName}";

            try
            {
                await _s3Client.GetObjectMetadataAsync(_bucketName, key);
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = true
                };
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = true,
                        Message = "Record retrieved successfully",
                        Data = false
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
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
