using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssessmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAssessmentAsync(PatientAssessment assessment)
        {
            await _context.PatientAssessments.AddAsync(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task AddPatientAnswerAsync(PatientAnswers answer)
        {
            await _context.PatientAnswers.AddAsync(answer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PatientAssessment>> GetPatientProgressAsync(int patientId)
        {
            return await _context.PatientAssessments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
