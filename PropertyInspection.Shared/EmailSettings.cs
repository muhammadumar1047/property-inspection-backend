namespace PropertyInspection.Shared
{
    /// <summary>
    /// SMTP email configuration settings bound from appsettings.json "Email" section.
    /// </summary>
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPass { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
    }
}