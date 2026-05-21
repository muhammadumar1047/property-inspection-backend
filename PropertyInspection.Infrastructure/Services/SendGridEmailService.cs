using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using PropertyInspection.Core.Interfaces.Services;
using PropertyInspection.Infrastructure.Auth;

namespace PropertyInspection.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridSettings _settings;
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IOptions<SendGridSettings> options, ILogger<SendGridEmailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _logger.LogError("SendGrid ApiKey is not configured.");
                throw new InvalidOperationException("SendGrid ApiKey is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                _logger.LogError("SendGrid FromEmail is not configured.");
                throw new InvalidOperationException("SendGrid FromEmail is not configured.");
            }

            // SendGridClient is designed to be a singleton/reusable instance.
            // We instantiate it once here for the duration of this service lifetime.
            _sendGridClient = new SendGridClient(_settings.ApiKey);
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));
            }

            _logger.LogInformation("Attempting to send email to {Recipient} with subject: {Subject}", to, subject);

            try
            {
                var fromAddress = new EmailAddress(_settings.FromEmail, _settings.FromName ?? "Property Inspection");
                var toAddress = new EmailAddress(to);
                
                // Using the body for both html and plain text
                var plainTextContent = body;
                var htmlContent = body;

                var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, plainTextContent, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {Recipient}. Status Code: {StatusCode}", to, response.StatusCode);
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("Failed to send email to {Recipient}. Status Code: {StatusCode}. Response: {Response}", 
                        to, response.StatusCode, responseBody);
                    throw new Exception($"Failed to send email. SendGrid returned status code: {response.StatusCode}. Details: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {Recipient}.", to);
                throw;
            }
        }
    }
}
