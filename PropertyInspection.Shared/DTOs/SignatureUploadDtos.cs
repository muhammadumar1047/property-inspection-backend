namespace PropertyInspection.Shared.DTOs
{
    public class GenerateSignatureUploadUrlRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class SignatureUploadUrlResponse
    {
        public string PreSignedUrl { get; set; } = string.Empty;
        public string FileKey { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }

    public class SaveSignatureRequest
    {
        public string FileKey { get; set; } = string.Empty;
    }
}