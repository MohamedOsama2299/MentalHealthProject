using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CBTClinic.API.Controllers
{
    [ApiController]
    [Route("api/patient/quizzes")]
    public class PatientQuizController : ControllerBase
    {
        private readonly QuizService _quizService;
        private readonly IPatientRepository _patientRepository;

        public PatientQuizController(QuizService quizService, IPatientRepository patientRepository)
        {
            _quizService = quizService;
            _patientRepository = patientRepository;
        }



        private async Task<int?> GetPatientIdFromClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            var patientId = await _patientRepository.GetPatientIdByUserIdAsync(userId);
            return patientId;
        }
        


        // Get quiz with total score for the patient
        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuiz(int quizId)
        {
            var patientId = await GetPatientIdFromClaims();
            if (patientId == null)
                return Unauthorized(new { message = "Patient is not authenticated." });

            var quiz = await _quizService.GetQuizDetails(quizId);
            if (quiz == null)
                return NotFound(new { message = "Quiz not found or not published." });

            // Get patient's total score for this quiz
            var patientProgress = await _quizService.GetPatientProgress(patientId.Value);
            var totalScore = patientProgress.FirstOrDefault(p => p.QuizId == quizId)?.TotalScore ?? 0;

            return Ok(new
            {
                message = "Quiz retrieved successfully",
                quiz,
                totalScore
            });
        }



        // Submit quiz answers
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz(SubmitQuizDto dto)
        {
            var patientId = await GetPatientIdFromClaims();
            if (patientId == null)
                return Unauthorized(new { message = "Patient is not authenticated." });

            if (dto == null || dto.QuizId <= 0 || dto.Answers == null || !dto.Answers.Any())
                return BadRequest(new { message = "Invalid quiz submission data." });

            try
            {
                var result = await _quizService.SubmitQuiz(patientId.Value, dto);
                return Ok(new { message = "Quiz submitted successfully", result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error submitting quiz", detail = ex.Message });
            }
        }



        // Get all progress for the patient
        [HttpGet("progress")]
        public async Task<IActionResult> GetProgress()
        {
            var patientId = await GetPatientIdFromClaims();
            if (patientId == null)
                return Unauthorized(new { message = "Patient is not authenticated." });

            var progress = await _quizService.GetPatientProgress(patientId.Value);

            var groupedProgress = progress
                .GroupBy(p => p.QuizId)
                .Select(g => new
                {
                    QuizId = g.Key,
                    TotalScore = g.Sum(p => p.TotalScore), 
                    SeverityLevel = CalculateSeverity(g.Sum(p => p.TotalScore)), 
                    LastAttemptAt = g.Max(p => p.CreatedAt) 
                })
                .ToList();

            return Ok(new { message = "Patient progress retrieved successfully", progress = groupedProgress });
        }

        private string CalculateSeverity(int totalScore)
        {
            if (totalScore <= 4) return "Minimal";
            if (totalScore <= 9) return "Mild";
            if (totalScore <= 14) return "Moderate";
            if (totalScore <= 19) return "Moderately Severe";
            return "Severe";
        }
    }
}