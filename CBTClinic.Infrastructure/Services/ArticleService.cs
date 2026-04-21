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
    public class ArticleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticleService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task AddArticleAsync(int adminId, CreateArticleDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("File is required");

            if (Path.GetExtension(dto.File.FileName) != ".pdf")
                throw new Exception("Only PDF allowed");


            var folderPath = Path.Combine(_env.WebRootPath, "articles");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            
            var article = new Article
            {
                Title = dto.Title,
                FilePath = "/articles/" + fileName,
                AdminId = adminId
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _context.Articles
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, CreateArticleDto dto)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
                throw new Exception("Article not found");

            if (!string.IsNullOrEmpty(dto.Title))
                article.Title = dto.Title;

            if (dto.File != null)
            {
                var extension = Path.GetExtension(dto.File.FileName).ToLower();

                if (extension != ".pdf")
                    throw new Exception("Only PDF allowed");

                var oldPath = Path.Combine(_env.WebRootPath, article.FilePath.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                var folderPath = Path.Combine(_env.WebRootPath, "articles");

                var fileName = Guid.NewGuid() + extension;
                var newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                article.FilePath = "/articles/" + fileName;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _context.Articles
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

            keyword = keyword.ToLower();

            return await _context.Articles
                .Where(a => a.Title.ToLower().Contains(keyword))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
                throw new Exception("Article not found");

            var fullPath = Path.Combine(_env.WebRootPath, article.FilePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
    }
}