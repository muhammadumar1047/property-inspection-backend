using PropertyInspection.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Interfaces.Repositories
{
    public interface IReportSyncRepository
    {
        Task<bool> SyncReportAsync(Report report);
    }
}
