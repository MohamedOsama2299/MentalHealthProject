using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Repositories
{
    public class OptionRepository : IOptionRepository
    {
        private readonly ApplicationDbContext _context;

        public OptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Option?> GetByIdAsync(int id)
        {
            return await _context.Options.FindAsync(id);
        }

        public async Task<List<Option>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Options
                .Where(o => o.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task AddAsync(Option option)
        {
            await _context.Options.AddAsync(option);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Option option)
        {
            _context.Options.Update(option);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Option option)
        {
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
        }
    }
}
