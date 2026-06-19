using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    /// <summary>
    /// Service for handling customer support requests.
    /// </summary>
    public interface ISupportService
    {
        /// <summary>
        /// Submits a support request by sending an email to the support team via SMTP.
        /// </summary>
        Task<ServiceResponse<bool>> SubmitSupportRequestAsync(CreateSupportRequestDto dto);
    }
}