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
    public class PodcastService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PodcastService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task AddPodcastAsync(int adminId, CreatePodcastDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("File is required");

            var extension = Path.GetExtension(dto.File.FileName).ToLower();

            if (extension != ".mp3" && extension != ".m4a")
                throw new Exception("Only MP3 or M4A allowed");

            var folderPath = Path.Combine(_env.WebRootPath, "podcasts");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var podcast = new Podcast
            {
                Title = dto.Title,
                FilePath = "/podcasts/" + fileName,
                AdminId = adminId
            };

            _context.Podcasts.Add(podcast);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Podcast>> GetAllAsync()
        {
            return await _context.Podcasts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, CreatePodcastDto dto)
        {
            var article = await _context.Podcasts.FindAsync(id);

            if (article == null)
                throw new Exception("Podcast not found");

            if (!string.IsNullOrEmpty(dto.Title))
                article.Title = dto.Title;

            if (dto.File != null)
            {
                var extension = Path.GetExtension(dto.File.FileName).ToLower();

                if (extension != ".mp3" && extension != ".m4a")
                    throw new Exception("Only MP3 or M4A allowed");

                var oldPath = Path.Combine(_env.WebRootPath, article.FilePath.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                var folderPath = Path.Combine(_env.WebRootPath, "Podcasts");

                var fileName = Guid.NewGuid() + extension;
                var newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                article.FilePath = "/Podcast/" + fileName;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var podcast = await _context.Podcasts.FindAsync(id);

            if (podcast == null)
                throw new Exception("Podcast not found");

            var fullPath = Path.Combine(_env.WebRootPath, podcast.FilePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Podcasts.Remove(podcast);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Podcast>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.Podcasts
                .Where(p => EF.Functions.Like(p.Title, $"%{keyword}%"))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}