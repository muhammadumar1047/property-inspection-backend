using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PropertyInspection.Application.IServices;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3StorageService> _logger;

        public S3StorageService(IAmazonS3 s3Client, IConfiguration configuration, ILogger<S3StorageService> logger)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadLogoAsync(IFormFile file, string agencyId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var bucketName = _configuration["AWS:S3Bucket"];
            if (string.IsNullOrEmpty(bucketName))
                throw new InvalidOperationException("AWS:S3Bucket configuration is missing.");

            // Basic validation
            if (file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("File size exceeds 2MB limit.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".svg")
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and SVG are allowed.");

            var objectKey = $"logos/{agencyId}/{Guid.NewGuid()}{extension}";

            try
            {
                using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);
                newMemoryStream.Position = 0;

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = objectKey,
                    BucketName = bucketName,
                    ContentType = file.ContentType
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);

                // Construct public URL. Note: Depending on region, the URL format might differ slightly.
                // Standard format: https://{bucketName}.s3.amazonaws.com/{objectKey}
                // or https://{bucketName}.s3.{region}.amazonaws.com/{objectKey}
                // We'll use the region endpoint from the client if available, else fallback to standard
                var region = _s3Client.Config.RegionEndpoint?.SystemName ?? "us-east-1";
                var url = $"https://{bucketName}.s3.{region}.amazonaws.com/{objectKey}";

                return url;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "S3 upload failed. Bucket:'{Bucket}', Key:'{Key}', Message:'{Message}'", bucketName, objectKey, e.Message);
                throw new InvalidOperationException($"Failed to upload file to S3: {e.Message}", e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown encountered on server. Message:'{Message}' when writing an object", e.Message);
                throw new Exception("An unknown error occurred during file upload.", e);
            }
        }
    }
}
