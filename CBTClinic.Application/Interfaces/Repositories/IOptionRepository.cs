using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Domain.Entities;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    using CBTClinic.Application.DTOs.Quiz;
    using CBTClinic.Application.DTOs.Question;
    using CBTClinic.Application.DTOs.Option;

    public interface IOptionRepository
    {
        Task<Option?> GetByIdAsync(int id);

        Task<List<Option>> GetByQuestionIdAsync(int questionId);

        Task AddAsync(Option option);

        Task UpdateAsync(Option option);

        Task DeleteAsync(Option option);
    }

}
