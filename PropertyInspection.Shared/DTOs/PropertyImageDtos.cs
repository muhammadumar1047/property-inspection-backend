using System;

namespace PropertyInspection.Shared.DTOs
{
    public class PropertyImageUploadResponse
    {
        public string ImageGuid { get; set; } = string.Empty;
        public string FileKey { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
