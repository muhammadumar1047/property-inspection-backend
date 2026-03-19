using PropertyInspection.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Shared.DTOs
{
    public class GenerateUploadUrlsRequest
    {
        [Required]
        public Guid? AgencyId { get; set; }

        [Required]
        public Guid? PropertyId { get; set; }

        [Required]
        public Guid? InspectionId { get; set; }

        [Required]
        public MediaType MediaType { get; set; }

        [Required]
        public List<UploadFileRequest> Files { get; set; } = new();
    }

    public class UploadFileRequest
    {
        [Required]
        public string ContentType { get; set; } = string.Empty;

        public string? OriginalFileName { get; set; }
    }

    public class UploadFileResponse
    {
        public string UploadUrl { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileKey { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? OriginalFileName { get; set; }
    }
}
