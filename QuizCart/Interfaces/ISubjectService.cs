using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> ListSubjects();

        Task<SubjectDto?> FindSubject(int id);

        Task<ServiceResponse> AddSubject(AddSubjectDto addSubjectDto);

        Task<ServiceResponse> UpdateSubject(int id, UpdateSubjectDto updateSubjectDto);
        Task<ServiceResponse> DeleteSubject(int id);
        Task<IEnumerable<SubjectDto>> ListSubjectsByMemberId(int memberId);

        Task<PaginatedResult<SubjectDto>> GetPaginatedSubjects(int page, int pageSize);


    }
}
