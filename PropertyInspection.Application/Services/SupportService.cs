using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System.Net;
using System.Net.Mail;

namespace PropertyInspection.Application.Services
{
    public class SupportService : ISupportService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SupportService> _logger;

        public SupportService(IOptions<EmailSettings> options, ILogger<SupportService> logger)
        {
            _emailSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<bool>> SubmitSupportRequestAsync(CreateSupportRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                if (string.IsNullOrWhiteSpace(_emailSettings.SupportEmail))
                {
                    _logger.LogError("Support email is not configured");
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Support email is not configured on the server",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }

                var subject = $"Support Request: {dto.Subject}";
                var body = BuildEmailBody(dto);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass)
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SmtpUser, "EaseInspect Support"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(_emailSettings.SupportEmail);
                mailMessage.ReplyToList.Add(new MailAddress(dto.Email, dto.Name));

                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Support request from {Name} ({Email}) sent successfully", dto.Name, dto.Email);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Your support request has been submitted successfully. We will get back to you shortly.",
                    Data = true
                };
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error while sending support request from {Email}", dto?.Email);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Failed to send your support request. Please try again later.",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing support request from {Email}", dto?.Email);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later.",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private static string BuildEmailBody(CreateSupportRequestDto dto)
        {
            var phoneSection = !string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? $"<tr><td style='padding:8px 0;color:#6b7280;font-size:13px'>Phone</td><td style='padding:8px 0'>{WebUtility.HtmlEncode(dto.PhoneNumber)}</td></tr>"
                : string.Empty;

            return $@"<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family:-apple-system,BlinkMacSystemFont,sans-serif;background:#f3f4f6;margin:0;padding:24px'>
  <div style='max-width:600px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 1px 3px rgba(0,0,0,0.1)'>
    <div style='background:#4f46e5;padding:24px;color:#fff'>
      <h2 style='margin:0;font-size:18px'>New Support Request</h2>
    </div>
    <div style='padding:24px'>
      <table style='width:100%;border-collapse:collapse;font-size:14px'>
        <tr><td style='padding:8px 0;color:#6b7280;font-size:13px'>Name</td><td style='padding:8px 0'>{WebUtility.HtmlEncode(dto.Name)}</td></tr>
        <tr><td style='padding:8px 0;color:#6b7280;font-size:13px'>Email</td><td style='padding:8px 0'><a href='mailto:{WebUtility.HtmlEncode(dto.Email)}'>{WebUtility.HtmlEncode(dto.Email)}</a></td></tr>
        {phoneSection}
        <tr><td style='padding:8px 0;color:#6b7280;font-size:13px'>Subject</td><td style='padding:8px 0'>{WebUtility.HtmlEncode(dto.Subject)}</td></tr>
      </table>
      <div style='margin-top:16px;padding:16px;background:#f9fafb;border-radius:8px;border-left:4px solid #4f46e5'>
        <p style='margin:0;color:#6b7280;font-size:13px'>Message</p>
        <p style='margin:8px 0 0;white-space:pre-wrap;font-size:14px'>{WebUtility.HtmlEncode(dto.Message)}</p>
      </div>
    </div>
    <div style='padding:16px 24px;background:#f9fafb;color:#9ca3af;font-size:12px;text-align:center'>
      This email was sent via the EaseInspect Customer Support API
    </div>
  </div>
</body>
</html>";
        }
    }
}