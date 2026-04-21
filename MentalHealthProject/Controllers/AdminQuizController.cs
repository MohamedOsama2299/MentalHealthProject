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
    [Route("api/admin/quizzes")]
   [Authorize(Roles = "Admin")] 
    public class AdminQuizController : ControllerBase
    {
        private readonly QuizService _quizService;

        public AdminQuizController(QuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz(CreateQuizDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });

            var id = await _quizService.CreateQuiz(dto);
            return Ok(new { Message = "Quiz created successfully", QuizId = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, UpdateQuizDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });

            await _quizService.UpdateQuiz(id, dto);
            return Ok(new { Message = "Quiz updated successfully" });
        }

        [HttpPost("questions")]
        public async Task<IActionResult> AddQuestion(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });

            var id = await _quizService.AddQuestionAsync(dto);
            return Ok(new { Message = "Question added successfully", QuestionId = id });
        }

        [HttpPut("questions/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });

            await _quizService.UpdateQuestion(id, dto);
            return Ok(new { Message = "Question updated successfully" });
        }
    }
}