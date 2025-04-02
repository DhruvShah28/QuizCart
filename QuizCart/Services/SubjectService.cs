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

        /// <summary>
        /// Returns a list of subjects including their total assessments and members.
        /// </summary>
        /// <returns>
        /// List of SubjectDto containing SubjectId, Name, Description, TotalAssessments, and TotalMembers
        ///</returns>


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

        /// <summary>
        /// Finds and returns a subject by its ID with its assessments and members.
        /// </summary>
        /// <param name="id">The ID of the subject</param>
        /// <returns>SubjectDto with subject details or null if not found</returns>

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

        /// <summary>
        /// Adds a new subject to the database.
        /// </summary>
        /// <param name="addDto">The subject data to add</param>
        /// <returns>ServiceResponse with status and created subject ID or error message</returns>

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

        /// <summary>
        /// Updates an existing subject.
        /// </summary>
        /// <param name="id">The ID of the subject to update</param>
        /// <param name="dto">The updated subject details</param>
        /// <returns>ServiceResponse with status indicating success, error, or not found</returns>

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

        /// <summary>
        /// Deletes a subject by ID.
        /// </summary>
        /// <param name="id">The ID of the subject to delete</param>
        /// <returns>ServiceResponse with status indicating success, error, or not found</returns>

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

        /// <summary>
        /// Lists subjects that a given member is associated with, including assessment and member counts.
        /// </summary>
        /// <param name="memberId">The ID of the member</param>
        /// <returns>List of SubjectDto or an empty list if member or subjects are not found</returns>

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
