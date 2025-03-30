using QuizCart.Models;

namespace QuizCart.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberDto>> ListMembers();

        Task<MemberDto?> FindMember(int id);

        Task<ServiceResponse> UpdateMember(int id, UpdateMemberDto UpdateMemberDto);

        Task<ServiceResponse> AddMember(AddMemberDto AddMemberDto);

        Task<ServiceResponse> DeleteMember(int id);

        Task<ServiceResponse> LinkSubject(LinkSubjectDto dto);
        Task<ServiceResponse> UnlinkSubject(LinkSubjectDto dto);
    }
}
