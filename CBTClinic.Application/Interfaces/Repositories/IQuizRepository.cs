using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Domain.Entities;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    using CBTClinic.Application.DTOs.Quiz;
    using CBTClinic.Application.DTOs.Question;
    using CBTClinic.Application.DTOs.Option;

    public interface IQuizRepository
    {
        Task<List<Quizzes>> GetAllAsync();

        Task<Quizzes?> GetByIdAsync(int id);

        Task<Quizzes?> GetQuizWithQuestionsAsync(int quizId);

        Task AddAsync(Quizzes quiz);

        Task UpdateAsync(Quizzes quiz);

        Task DeleteAsync(Quizzes quiz);
    }

}
