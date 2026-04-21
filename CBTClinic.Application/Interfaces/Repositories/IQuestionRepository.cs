using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Domain.Entities;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    using CBTClinic.Application.DTOs.Quiz;
    using CBTClinic.Application.DTOs.Question;
    using CBTClinic.Application.DTOs.Option;

    public interface IQuestionRepository
    {
        Task<Questions?> GetByIdAsync(int id);

        Task<List<Questions>> GetByQuizIdAsync(int quizId);

        Task AddAsync(Questions question);

        Task UpdateAsync(Questions question);

        Task DeleteAsync(Questions question);
    }

}
