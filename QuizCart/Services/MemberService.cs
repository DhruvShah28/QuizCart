using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;

        public MemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all members along with their financial information, subject count, and assessment count.
        /// Calculates each member's amount paid, amount owed (based on shared expense), and counts total subjects and assessments.
        /// </summary>
        /// <returns>List of MemberDto containing detailed member info.</returns>
        public async Task<IEnumerable<MemberDto>> ListMembers()
        {
            var members = await _context.Members
                .Include(m => m.Purchases)
                    .ThenInclude(p => p.BrainFoods)
                        .ThenInclude(bf => bf.Ingredient)
                .Include(m => m.Subjects)
                    .ThenInclude(s => s.Assessments)
                .ToListAsync();

            if (members.Count == 0) return [];

            int totalMembers = members.Count;

            float totalSpent = members
                .SelectMany(m => m.Purchases)
                .SelectMany(p => p.BrainFoods)
                .Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice);

            var memberDtos = members.Select(m =>
            {
                float amountPaid = m.Purchases
                    .SelectMany(p => p.BrainFoods)
                    .Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice);

                float originalOwed = totalSpent / totalMembers;
                float amountOwed = originalOwed - amountPaid;

                return new MemberDto
                {
                    MemberId = m.MemberId,
                    Name = m.Name,
                    Email = m.Email, 
                    AmountPaid = amountPaid,
                    AmountOwed = amountOwed,
                    TotalSubjects = m.Subjects.Count,
                    TotalAssessments = m.Subjects.SelectMany(s => s.Assessments).Count()
                };
            }).ToList();

            return memberDtos;
        }

        /// <summary>
        /// Retrieves a single member by their ID and calculates their payment/owes details and counts.
        /// </summary>
        /// <param name="id">The ID of the member to retrieve.</param>
        /// <returns>MemberDto with the member's details or null if not found.</returns>
        public async Task<MemberDto?> FindMember(int id)
        {
            var members = await _context.Members
                .Include(m => m.Purchases)
                    .ThenInclude(p => p.BrainFoods)
                        .ThenInclude(bf => bf.Ingredient)
                .Include(m => m.Subjects)
                    .ThenInclude(s => s.Assessments)
                .ToListAsync();

            var targetMember = members.FirstOrDefault(m => m.MemberId == id);
            if (targetMember == null) return null;

            int totalMembers = members.Count;

            float totalSpent = members
                .SelectMany(m => m.Purchases)
                .SelectMany(p => p.BrainFoods)
                .Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice);

            float amountPaid = targetMember.Purchases
                .SelectMany(p => p.BrainFoods)
                .Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice);

            float originalOwed = totalSpent / totalMembers;
            float amountOwed = originalOwed - amountPaid;

            return new MemberDto
            {
                MemberId = targetMember.MemberId,
                Name = targetMember.Name,
                Email = targetMember.Email, 
                AmountPaid = amountPaid,
                AmountOwed = amountOwed,
                TotalSubjects = targetMember.Subjects.Count,
                TotalAssessments = targetMember.Subjects.SelectMany(s => s.Assessments).Count()
            };
        }

        /// <summary>
        /// Updates a member's name and email based on the given ID and update DTO.
        /// </summary>
        /// <param name="id">The ID of the member to update.</param>
        /// <param name="updateDto">The updated member details.</param>
        /// <returns>ServiceResponse indicating the result of the operation.</returns>

        public async Task<ServiceResponse> UpdateMember(int id, UpdateMemberDto updateDto)
        {
            ServiceResponse serviceResponse = new();


            if (id != updateDto.MemberId)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Member ID mismatch.");
                return serviceResponse;
            }


            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Member not found.");
                return serviceResponse;
            }


            member.Name = updateDto.Name;
            member.Email = updateDto.Email;


            _context.Entry(member).State = EntityState.Modified;

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;

                if (!await _context.Members.AnyAsync(m => m.MemberId == id))
                {
                    serviceResponse.Messages.Add("Member not found after concurrency check.");
                    serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                }
                else
                {
                    serviceResponse.Messages.Add("An error occurred while updating the member.");
                }

                return serviceResponse;
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }

        /// <summary>
        /// Adds a new member to the database.
        /// </summary>
        /// <param name="addDto">The data for the new member.</param>
        /// <returns>ServiceResponse containing status and created ID if successful.</returns>

        public async Task<ServiceResponse> AddMember(AddMemberDto addDto)
        {
            ServiceResponse serviceResponse = new();


            Member member = new Member()
            {
                Name = addDto.Name,
                Email = addDto.Email
            };

            try
            {

                _context.Members.Add(member);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Member.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = member.MemberId;
            return serviceResponse;
        }

        /// <summary>
        /// Deletes a member from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the member to delete.</param>
        /// <returns>ServiceResponse indicating the result of the operation.</returns>

        public async Task<ServiceResponse> DeleteMember(int id)
        {
            ServiceResponse serviceResponse = new();


            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {

                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Member cannot be deleted because it does not exist.");
                return serviceResponse;
            }

            try
            {

                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the member.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

        /// <summary>
        /// Links a subject to a member if not already linked.
        /// </summary>
        /// <param name="dto">DTO containing MemberId and SubjectId to link.</param>
        /// <returns>ServiceResponse indicating result.</returns>

        public async Task<ServiceResponse> LinkSubject(LinkSubjectDto dto)
        {
            var response = new ServiceResponse();

            var member = await _context.Members
                .Include(m => m.Subjects)
                .FirstOrDefaultAsync(m => m.MemberId == dto.MemberId);

            var subject = await _context.Subjects.FindAsync(dto.SubjectId);

            if (member == null || subject == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Member or Subject not found.");
                return response;
            }

            if (!member.Subjects.Contains(subject))
            {
                member.Subjects.Add(subject);
                await _context.SaveChangesAsync();
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        /// <summary>
        /// Unlinks a subject from a member if currently linked.
        /// </summary>
        /// <param name="dto">DTO containing MemberId and SubjectId to unlink.</param>
        /// <returns>ServiceResponse indicating result.</returns>

        public async Task<ServiceResponse> UnlinkSubject(LinkSubjectDto dto)
        {
            var response = new ServiceResponse();

            var member = await _context.Members
                .Include(m => m.Subjects)
                .FirstOrDefaultAsync(m => m.MemberId == dto.MemberId);

            if (member == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Member not found.");
                return response;
            }

            var subject = member.Subjects.FirstOrDefault(s => s.SubjectId == dto.SubjectId);

            if (subject == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Subject is not linked to the member.");
                return response;
            }

            member.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }


        public async Task<PaginatedResult<MemberDto>> GetPaginatedMembers(int page, int pageSize)
        {
            var query = _context.Members
                .Include(m => m.Subjects)
                .Include(m => m.Purchases)
                    .ThenInclude(p => p.BrainFoods)
                        .ThenInclude(bf => bf.Ingredient)
                .Include(m => m.Subjects!)
                    .ThenInclude(s => s.Assessments)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var members = await query
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = members.Select(m => new MemberDto
            {
                MemberId = m.MemberId,
                Name = m.Name,
                Email = m.Email,
                TotalSubjects = m.Subjects?.Count ?? 0,
                TotalAssessments = m.Subjects?.SelectMany(s => s.Assessments).Count() ?? 0,
                AmountPaid = 0, // You can populate this if needed
                AmountOwed = m.Purchases.Sum(p => p.BrainFoods.Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice))
            }).ToList();

            return new PaginatedResult<MemberDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}
