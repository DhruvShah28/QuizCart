using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;

        public SubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectDto>> ListSubjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Assessments)
                .Include(s => s.Members)
                .ToListAsync();

            return subjects.Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                Description = s.Description,
                TotalAssessments = s.Assessments.Count,
                TotalMembers = s.Members.Count
            }).ToList();
        }

        public async Task<SubjectDto?> FindSubject(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Assessments)
                .Include(s => s.Members)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null) return null;

            return new SubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                Description = subject.Description,
                TotalAssessments = subject.Assessments.Count,
                TotalMembers = subject.Members.Count
            };
        }

        public async Task<ServiceResponse> AddSubject(AddSubjectDto addDto)
        {
            ServiceResponse response = new();

            var subject = new Subject
            {
                Name = addDto.Name,
                Description = addDto.Description
            };

            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = subject.SubjectId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding subject.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateSubject(int id, UpdateSubjectDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.SubjectId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Subject ID mismatch.");
                return response;
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Subject not found.");
                return response;
            }

            subject.Name = dto.Name;
            subject.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating subject.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteSubject(int id)
        {
            ServiceResponse response = new();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Subject not found.");
                return response;
            }

            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting subject.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }



        public async Task<IEnumerable<SubjectDto>> ListSubjectsByMemberId(int memberId)
        {
            var member = await _context.Members
                .Include(m => m.Subjects)
                .ThenInclude(s => s.Assessments)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null || member.Subjects == null)
                return [];

            return member.Subjects.Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                Description = s.Description,
                TotalAssessments = s.Assessments.Count,
                TotalMembers = s.Members?.Count ?? 0
            }).ToList();
        }


    }
}
