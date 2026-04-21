using CBTClinic.Application.DTOs.Content;
using CBTClinic.Application.DTOs.Question;
using CBTClinic.Application.DTOs.Quiz;
using CBTClinic.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CBTClinic.API.Controllers
{
    [ApiController]
    [Route("api/admin/articles")]
    [Authorize(Roles = "Admin")]
    public class AdminArticleController : ControllerBase
    {
        private readonly ArticleService _service;

        public AdminArticleController(ArticleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateArticleDto dto)
        {
            var adminId = int.Parse(User.FindFirst("AdminId").Value);

            await _service.AddArticleAsync(adminId, dto);

            return Ok(new { message = "Article uploaded successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();

            if (data == null || !data.Any())
                return NotFound(new { message = "No articles found" });

            return Ok(data.Select(a => new
            {
                a.Id,
                a.Title,
                a.FilePath,
                a.CreatedAt
            }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateArticleDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new { message = "Updated successfully" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var data = await _service.SearchAsync(keyword);

            if (data == null || !data.Any())
                return NotFound(new { message = "No matching articles found" });

            return Ok(data.Select(a => new
            {
                a.Id,
                a.Title,
                a.FilePath,
                a.CreatedAt
            }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new { message = "Deleted successfully" });
        }
    }
}