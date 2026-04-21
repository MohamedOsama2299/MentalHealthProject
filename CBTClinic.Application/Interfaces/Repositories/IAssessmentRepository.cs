using CBTClinic.Application.DTOs.Assessment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Domain.Entities;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    using CBTClinic.Application.DTOs.Quiz;
    using CBTClinic.Application.DTOs.Question;
    using CBTClinic.Application.DTOs.Option;

    public interface IAssessmentRepository
    {
        Task AddAssessmentAsync(PatientAssessment assessment);

        Task AddPatientAnswerAsync(PatientAnswers answer);

        Task<List<PatientAssessment>> GetPatientProgressAsync(int patientId);
    }

}
