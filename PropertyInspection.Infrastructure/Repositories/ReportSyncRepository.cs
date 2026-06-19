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
                    // Remove old report (cascade-deletes all children) and save immediately
                    // to detach from the tracker so the PK is free for the new instance
                    _dbSet.Remove(existingReport);
                    await _context.SaveChangesAsync();

                    // Now add the new report with fresh GUIDs – no PK conflict
                    _dbSet.Add(report);
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
