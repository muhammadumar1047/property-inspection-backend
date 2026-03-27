using System;
using System.Threading.Tasks;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IBillingService
    {
        Task<ServiceResponse<BillingDto>> CreateBillingAsync(CreateBillingDto dto);
        Task<ServiceResponse<BillingDto>> UpdateBillingAsync(Guid id, UpdateBillingDto dto);
        Task<ServiceResponse<bool>> DeleteBillingAsync(Guid id);
        Task<ServiceResponse<bool>> ActivateBillingAsync(Guid id);
        Task<ServiceResponse<bool>> DeactivateBillingAsync(Guid id);
        Task<ServiceResponse<BillingDto>> GetBillingByIdAsync(Guid id);
        Task<ServiceResponse<PagedResult<BillingDto>>> GetBillingsAsync(BillingFilterDto filter);
    }
}
