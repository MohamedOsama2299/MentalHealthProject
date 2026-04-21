using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Assessment.Patient;
using CBTClinic.Application.DTOs.Option;
using CBTClinic.Application.DTOs.Question;
using CBTClinic.Application.DTOs.Quiz;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;

namespace CBTClinic.Infrastructure.Services
{
    public class QuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IOptionRepository _optionRepository;
        private readonly IAssessmentRepository _assessmentRepository;

        public QuizService(
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository,
            IOptionRepository optionRepository,
            IAssessmentRepository assessmentRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _optionRepository = optionRepository;
            _assessmentRepository = assessmentRepository;
        }

        // Create Quiz
        public async Task<int> CreateQuiz(CreateQuizDto dto)
        {
            var quiz = new Quizzes
            {
                Title = dto.Title,
                Description = dto.Description,
                IsPublished = dto.IsPublished,
                IsPaid = dto.IsPaid,
                Price = dto.Price
            };

            await _quizRepository.AddAsync(quiz);

            return quiz.Id;
        }

        // Update Quiz
        public async Task UpdateQuiz(int id, UpdateQuizDto dto)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);

            if (quiz == null)
                throw new Exception("Quiz not found");

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            quiz.IsPublished = dto.IsPublished;
            quiz.IsPaid = dto.IsPaid;
            quiz.Price = dto.Price;

            await _quizRepository.UpdateAsync(quiz);
        }

        // Add Question
        public async Task<int> AddQuestionAsync(CreateQuestionDto dto)
        {
            var question = new Questions
            {
                QuizId = dto.QuizId,
                Text = dto.Text,
                Order = dto.Order
            };

            await _questionRepository.AddAsync(question);

            // Auto Create Options
            var options = new List<Option>
            {
                new Option
                {
                    QuestionId = question.Id,
                    Text = "Not at all",
                    Score = 0
                },
                new Option
                {
                    QuestionId = question.Id,
                    Text = "Several days",
                    Score = 1
                },
                new Option
                {
                    QuestionId = question.Id,
                    Text = "More than half the days",
                    Score = 2
                },
                new Option
                {
                    QuestionId = question.Id,
                    Text = "Nearly every day",
                    Score = 3
                }
            };

            foreach (var option in options)
            {
                await _optionRepository.AddAsync(option);
            }

            return question.Id;
        }

        // Update Question
        public async Task UpdateQuestion(int id, UpdateQuestionDto dto)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
                throw new Exception("Question not found");

            question.Text = dto.Text;
            question.Order = dto.Order;

            await _questionRepository.UpdateAsync(question);
        }

        // Get Quiz For Patient
        public async Task<QuizDetailsDto?> GetQuizDetails(int quizId)
        {
            var quiz = await _quizRepository.GetQuizWithQuestionsAsync(quizId);

            if (quiz == null || !quiz.IsPublished)
                return null;

            return new QuizDetailsDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Order = q.Order,
                        Options = q.Options.Select(o => new OptionDto
                        {
                            Id = o.Id,
                            Text = o.Text
                        }).ToList()
                    }).ToList()
            };
        }

        // Submit Quiz
        public async Task<AssessmentResultDto> SubmitQuiz(int patientId, SubmitQuizDto dto)
        {
            int totalScore = 0;

            foreach (var answer in dto.Answers)
            {
                var option = await _optionRepository.GetByIdAsync(answer.SelectedOptionId);

                if (option == null)
                    continue;

                totalScore += option.Score;

                var patientAnswer = new PatientAnswers
                {
                    PatientId = patientId,
                    QuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId,
                     AnsweredAt = DateTime.UtcNow
                };

                await _assessmentRepository.AddPatientAnswerAsync(patientAnswer);
            }

            var severity = CalculateSeverity(totalScore);

            var assessment = new PatientAssessment
            {
                PatientId = patientId,
                QuizId = dto.QuizId,
                TotalScore = totalScore,
                SeverityLevel = severity
            };

            await _assessmentRepository.AddAssessmentAsync(assessment);

            return new AssessmentResultDto
            {
                TotalScore = totalScore,
                SeverityLevel = severity,
                CreatedAt = assessment.CreatedAt
            };
        }

        // Patient Progress
        public async Task<List<PatientProgressDto>> GetPatientProgress(int patientId)
        {
            var assessments = await _assessmentRepository.GetPatientProgressAsync(patientId);

            return assessments.Select(a => new PatientProgressDto
            {
                QuizId = a.QuizId,
                TotalScore = a.TotalScore,
                SeverityLevel = a.SeverityLevel,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        private string CalculateSeverity(int score)
        {
            if (score <= 4) return "Minimal";
            if (score <= 9) return "Mild";
            if (score <= 14) return "Moderate";
            if (score <= 19) return "Moderately Severe";

            return "Severe";
        }
    }
}