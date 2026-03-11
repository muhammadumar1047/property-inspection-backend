using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReportDto?> GetReportByInspectionIdAsync(Guid inspectionId , Guid? agencyId)
        {
            var report = await _unitOfWork.Reports.FirstOrDefaultAsync(
                r => r.InspectionId == inspectionId && r.AgencyId == agencyId,
                include: q => q
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.Property)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.Agency)
                            .ThenInclude(a => a.AgencyWhitelabel)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.Inspector)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.InspectionStatus)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.InspectionType)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.LandlordSnapshots)
                    .Include(r => r.Inspection)
                        .ThenInclude(i => i.TenancySnapshots)
                    .Include(r => r.ReportAreas)
                        .ThenInclude(area => area.ReportItems)
                            .ThenInclude(item => item.ReportItemConditions)
                    .Include(r => r.ReportAreas)
                        .ThenInclude(area => area.ReportItems)
                            .ThenInclude(item => item.ReportItemComments)
                    .Include(r => r.ReportAreas)
                        .ThenInclude(area => area.ReportItems)
                            .ThenInclude(item => item.ReportMedia)
                                .ThenInclude(media => media.ReportMediaComments));

            if (report == null)
                return null;

            return _mapper.Map<ReportDto>(report);
        }
    }
}
