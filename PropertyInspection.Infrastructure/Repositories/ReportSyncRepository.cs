using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Repositories
{
    public class ReportSyncRepository : GenericRepository<Report>, IReportSyncRepository
    {
        public ReportSyncRepository(AppDbContext context) : base(context) { }


        public async Task<bool> SyncReportAsync(Report report)
        {
            try
            {
                // Check if report exists
                var existingReport = await _dbSet
                    .Include(r => r.ReportAreas)
                        .ThenInclude(a => a.ReportItems)
                            .ThenInclude(i => i.ReportItemConditions)
                    .Include(r => r.ReportAreas)
                        .ThenInclude(a => a.ReportItems)
                            .ThenInclude(i => i.ReportItemComments)
                    .Include(r => r.ReportAreas)
                        .ThenInclude(a => a.ReportItems)
                            .ThenInclude(i => i.ReportMedia)
                                .ThenInclude(m => m.ReportMediaComments)
                    .FirstOrDefaultAsync(r => r.Id == report.Id);

                if (existingReport == null)
                {
                    _dbSet.Add(report);
                }
                else
                {
                    _dbSet.RemoveRange(existingReport);
                    existingReport.ReportAreas = report.ReportAreas;
                    existingReport.Notes = report.Notes;
                }



                // Update inspection status TODO to "Completed" 


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SyncReportAsync] Error: {ex}");
                return false;
            }
        }

    }
}
