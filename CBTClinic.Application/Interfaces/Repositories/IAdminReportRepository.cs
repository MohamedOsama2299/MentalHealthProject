using CBTClinic.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
 
 
        public interface IAdminReportRepository
        {
            Task<AdminReport> AddReportAsync(AdminReport report);
            Task<List<AdminReport>> GetReportsByAdminAsync(int adminId);
            Task<AdminReport?> GetByIdAsync(int id);
            Task UpdateReportAsync(AdminReport report);
            Task DeleteReportAsync(int id);
        }
    }


