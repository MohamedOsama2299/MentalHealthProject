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
    public class VideoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VideoService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task AddVideoAsync(int adminId, CreateVideoDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("File is required");

            if (dto.File.Length > 50 * 1024 * 1024)
                throw new Exception("Max size is 50MB");

            var extension = Path.GetExtension(dto.File.FileName).ToLower();

            if (extension != ".mp4")
                throw new Exception("Only MP4 allowed");

            var folderPath = Path.Combine(_env.WebRootPath, "videos");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var video = new Video
            {
                Title = dto.Title,
                FilePath = "/videos/" + fileName,
                AdminId = adminId
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Video>> GetAllAsync()
        {
            return await _context.Videos
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, CreateVideoDto dto)
        {
            var article = await _context.Videos.FindAsync(id);

            if (article == null)
                throw new Exception("Video not found");

            if (!string.IsNullOrEmpty(dto.Title))
                article.Title = dto.Title;

            if (dto.File != null)
            {
                var extension = Path.GetExtension(dto.File.FileName).ToLower();

                if (dto.File == null || dto.File.Length == 0)
                    throw new Exception("File is required");

                if (dto.File.Length > 50 * 1024 * 1024)
                    throw new Exception("Max size is 50MB");

                var oldPath = Path.Combine(_env.WebRootPath, article.FilePath.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                var folderPath = Path.Combine(_env.WebRootPath, "Videos");

                var fileName = Guid.NewGuid() + extension;
                var newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                article.FilePath = "/Videos/" + fileName;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);

            if (video == null)
                throw new Exception("Video not found");

            var fullPath = Path.Combine(_env.WebRootPath, video.FilePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Video>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.Videos
                .Where(v => EF.Functions.Like(v.Title, $"%{keyword}%"))
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }
    }
}