using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Interfaces
{
    public interface IAssessmentService
    {
        Task<IEnumerable<AssessmentDto>> ListAssessments();
        Task<AssessmentDto?> FindAssessment(int id);
        Task<ServiceResponse> AddAssessment(AddAssessmentDto addDto);
        Task<ServiceResponse> UpdateAssessment(int id, UpdateAssessmentDto updateDto);
        Task<ServiceResponse> DeleteAssessment(int id);
        Task<IEnumerable<AssessmentDto>> ListAssessmentsBySubjectId(int subjectId);
        Task<PaginatedResult<AssessmentDto>> GetPaginatedAssessments(int page, int pageSize);

    }
}
