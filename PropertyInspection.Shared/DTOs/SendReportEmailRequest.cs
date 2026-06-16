namespace PropertyInspection.Shared.DTOs
{
    public class SendReportEmailRequest
    {
        public string RecipientEmail { get; set; } = null!;

        /// <summary>
        /// Optional custom subject line. When provided, overrides the default subject.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Optional custom HTML body. When provided, overrides the default email body.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Optional email template identifier used to generate the email content.
        /// </summary>
        public string? TemplateId { get; set; }

        /// <summary>
        /// Optional sender email address. When provided, overrides the configured default from address.
        /// </summary>
        public string? SendFromEmail { get; set; }

        /// <summary>
        /// Optional sender display name. When provided, overrides the configured default from name.
        /// </summary>
        public string? SendFromName { get; set; }
    }
}