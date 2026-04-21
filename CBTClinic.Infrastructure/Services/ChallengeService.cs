using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Assessment.Patient;
using CBTClinic.Application.DTOs.Content;
using CBTClinic.Application.DTOs.Option;
using CBTClinic.Application.DTOs.Question;
using CBTClinic.Application.DTOs.Quiz;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CBTClinic.Infrastructure.Services
{
    public class ChallengeService
    {
        private readonly ApplicationDbContext _context;

        public ChallengeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(int adminId, CreateChallengeDto dto)
        {
            var challenge = new Challenge
            {
                Title = dto.Title,
                Description = dto.Description,
                Steps = dto.Steps,
                Benefits = dto.Benefits,
                Difficulty = dto.Difficulty,
                DurationDays = dto.DurationDays,
            };

            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Challenge>> GetAllAsync()
        {
            return await _context.Challenges
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Challenge>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.Challenges
                .Where(c => EF.Functions.Like(c.Title, $"%{keyword}%"))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, UpdateChallengeDto dto)
        {
            var challenge = await _context.Challenges.FindAsync(id);

            if (challenge == null)
                throw new Exception("Challenge not found");

            if (!string.IsNullOrEmpty(dto.Title))
                challenge.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                challenge.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Steps))
                challenge.Steps = dto.Steps;

            if (!string.IsNullOrEmpty(dto.Benefits))
                challenge.Benefits = dto.Benefits;

            if (dto.Difficulty.HasValue)
                challenge.Difficulty = dto.Difficulty.Value;

            if (dto.DurationDays.HasValue)
                challenge.DurationDays = dto.DurationDays.Value;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);

            if (challenge == null)
                throw new Exception("Challenge not found");

            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync();
        }
    }
}