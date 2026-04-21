using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Repositories
{
    public class AdminReportRepository : IAdminReportRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminReport> AddReportAsync(AdminReport report)
        {
            _context.AdminReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<List<AdminReport>> GetReportsByAdminAsync(int adminId)
        {
            return await _context.AdminReports
                .Include(r => r.Admin)
                .Where(r => r.AdminId == adminId)
                .ToListAsync();
        }

        public async Task<AdminReport> GetByIdAsync(int id)
        {
            return await _context.AdminReports
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateReportAsync(AdminReport report)
        {
            _context.AdminReports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReportAsync(int id)
        {
            var report = await GetByIdAsync(id);
            if (report != null)
            {
                _context.AdminReports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
    }
}