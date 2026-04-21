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
    [Route("api/admin/videos")]
    [Authorize(Roles = "Admin")]
    public class AdminVideoController : ControllerBase
    {
        private readonly VideoService _service;

        public AdminVideoController(VideoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateVideoDto dto)
        {
            var adminId = int.Parse(User.FindFirst("AdminId").Value);

            await _service.AddVideoAsync(adminId, dto);

            return Ok(new { message = "Video uploaded successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();

            if (data == null || !data.Any())
                return NotFound(new { message = "No videos found" });


            return Ok(data.Select(v => new
            {
                v.Id,
                v.Title,
                v.FilePath,
                v.CreatedAt
            }));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var data = await _service.SearchAsync(keyword);

            if (data == null || !data.Any())
                return NotFound(new { message = "No matching videos found" });

            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateVideoDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new { message = "Deleted successfully" });
        }
    }
}