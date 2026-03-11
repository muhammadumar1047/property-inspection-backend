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

        public Task<string> GeneratePresignedUploadUrlAsync(string folder, string fileName, string contentType)
        {
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
            return Task.FromResult(url);
        }

        public Task<string> GeneratePresignedDownloadUrlAsync(string folder, string fileName)
        {
            var key = $"{folder}/{fileName}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            var url = _s3Client.GetPreSignedURL(request);
            return Task.FromResult(url);
        }

        public async Task UploadFileAsync(Stream fileStream, string folder, string fileName, string contentType)
        {
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
        }

        public async Task DeleteFileAsync(string folder, string fileName)
        {
            var key = $"{folder}/{fileName}";

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task<bool> FileExistsAsync(string folder, string fileName)
        {
            var key = $"{folder}/{fileName}";

            try
            {
                await _s3Client.GetObjectMetadataAsync(_bucketName, key);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }   



    }
}
