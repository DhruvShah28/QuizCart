﻿using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ApplicationDbContext _context;

        public AssessmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all assessments including their associated subject, members, and brain foods with ingredient and purchase details.
        /// </summary>
        /// <returns>List of AssessmentDto representing detailed information about each assessment.</returns>


        public async Task<IEnumerable<AssessmentDto>> ListAssessments()
        {
            var assessments = await _context.Assessments
                .Include(a => a.Subject)
                    .ThenInclude(s => s.Members)
                .Include(a => a.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .Include(a => a.BrainFoods)
                    .ThenInclude(bf => bf.Purchases!)
                        .ThenInclude(p => p.Member)
                .ToListAsync();

            return assessments.Select(a => new AssessmentDto
            {
                AssessmentId = a.AssessmentId,
                Title = a.Title,
                Description = a.Description,
                DateOfAssessment = a.DateOfAssessment,
                DifficultyLevel = a.DifficultyLevel,
                SubjectName = a.Subject?.Name ?? "Unknown",
                MemberNames = a.Subject?.Members.Select(m => m.Name).ToList() ?? new(),
                BrainFoods = a.BrainFoods?.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    Quantity = bf.Quantity,
                    IngredientId = bf.IngredientId,
                    AssessmentId = bf.AssessmentId,
                    AssessmentName = a.Title,
                    IngredientName = bf.Ingredient?.Name ?? "Unknown",
                    Benefits = bf.Ingredient?.Benefits ?? "",
                    UnitPrice = bf.Ingredient?.UnitPrice ?? 0f,
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList();
        }

        /// <summary>
        /// Retrieves a single assessment based on its ID, with detailed subject, member, brain food, and ingredient data.
        /// </summary>
        /// <param name="id">The ID of the assessment to retrieve.</param>
        /// <returns>AssessmentDto object or null if not found.</returns>


        public async Task<AssessmentDto?> FindAssessment(int id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Subject)
                    .ThenInclude(s => s.Members)
                .Include(a => a.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .Include(a => a.BrainFoods)
                    .ThenInclude(bf => bf.Purchases!)
                        .ThenInclude(p => p.Member)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null) return null;

            return new AssessmentDto
            {
                AssessmentId = assessment.AssessmentId,
                Title = assessment.Title,
                Description = assessment.Description,
                DateOfAssessment = assessment.DateOfAssessment,
                DifficultyLevel = assessment.DifficultyLevel,
                SubjectName = assessment.Subject?.Name ?? "Unknown",
                MemberNames = assessment.Subject?.Members.Select(m => m.Name).ToList() ?? new(),
                BrainFoods = assessment.BrainFoods?.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    Quantity = bf.Quantity,
                    IngredientId = bf.IngredientId,
                    AssessmentId = bf.AssessmentId,
                    AssessmentName = assessment.Title,
                    IngredientName = bf.Ingredient?.Name ?? "Unknown",
                    Benefits = bf.Ingredient?.Benefits ?? "",
                    UnitPrice = bf.Ingredient?.UnitPrice ?? 0f,
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList() ?? new()
            };
        }

        /// <summary>
        /// Adds a new assessment to the database.
        /// </summary>
        /// <param name="dto">AddAssessmentDto containing data for the new assessment.</param>
        /// <returns>ServiceResponse with status and created ID if successful.</returns>


        public async Task<ServiceResponse> AddAssessment(AddAssessmentDto dto)
        {
            ServiceResponse response = new();

            var assessment = new Assessment
            {
                Title = dto.Title,
                Description = dto.Description,
                DateOfAssessment = dto.DateOfAssessment,
                DifficultyLevel = dto.DifficultyLevel,
                SubjectId = dto.SubjectId
            };

            try
            {
                _context.Assessments.Add(assessment);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = assessment.AssessmentId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding assessment.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing assessment's data.
        /// </summary>
        /// <param name="id">The ID of the assessment to update.</param>
        /// <param name="dto">UpdateAssessmentDto containing updated data.</param>
        /// <returns>ServiceResponse indicating the update result.</returns>


        public async Task<ServiceResponse> UpdateAssessment(int id, UpdateAssessmentDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.AssessmentId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Assessment ID mismatch.");
                return response;
            }

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Assessment not found.");
                return response;
            }

            assessment.Title = dto.Title;
            assessment.Description = dto.Description;
            assessment.DateOfAssessment = dto.DateOfAssessment;
            assessment.DifficultyLevel = dto.DifficultyLevel;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating assessment.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Deletes an assessment from the database.
        /// </summary>
        /// <param name="id">The ID of the assessment to delete.</param>
        /// <returns>ServiceResponse indicating the deletion result.</returns>


        public async Task<ServiceResponse> DeleteAssessment(int id)
        {
            ServiceResponse response = new();

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Assessment not found.");
                return response;
            }

            try
            {
                _context.Assessments.Remove(assessment);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting assessment.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Retrieves a list of assessments associated with a specific subject by subject ID.
        /// </summary>
        /// <param name="subjectId">The ID of the subject to filter assessments by.</param>
        /// <returns>List of AssessmentDto associated with the specified subject.</returns>


        public async Task<IEnumerable<AssessmentDto>> ListAssessmentsBySubjectId(int subjectId)
        {
            var subject = await _context.Subjects
                .Include(s => s.Members)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.BrainFoods!)
                        .ThenInclude(bf => bf.Ingredient)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.BrainFoods!)
                        .ThenInclude(bf => bf.Purchases!)
                            .ThenInclude(p => p.Member)
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId);

            if (subject == null || subject.Assessments == null)
                return [];

            return subject.Assessments.Select(a => new AssessmentDto
            {
                AssessmentId = a.AssessmentId,
                Title = a.Title,
                Description = a.Description,
                DateOfAssessment = a.DateOfAssessment,
                DifficultyLevel = a.DifficultyLevel,
                SubjectName = subject.Name,
                MemberNames = subject.Members?.Select(m => m.Name).ToList() ?? new(),
                BrainFoods = a.BrainFoods?.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    Quantity = bf.Quantity,
                    IngredientId = bf.IngredientId,
                    AssessmentId = bf.AssessmentId,
                    IngredientName = bf.Ingredient?.Name ?? "Unknown",
                    Benefits = bf.Ingredient?.Benefits ?? "",
                    UnitPrice = bf.Ingredient?.UnitPrice ?? 0f,
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList();
        }

        public async Task<PaginatedResult<AssessmentDto>> GetPaginatedAssessments(int page, int pageSize)
        {
            var query = _context.Assessments
                .Include(a => a.Subject)
                .Include(a => a.BrainFoods!)
                    .ThenInclude(bf => bf.Ingredient)
                .Include(a => a.BrainFoods!)
                    .ThenInclude(bf => bf.Purchases!)
                        .ThenInclude(p => p.Member)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var assessments = await query
                .OrderBy(a => a.DateOfAssessment)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = assessments.Select(a => new AssessmentDto
            {
                AssessmentId = a.AssessmentId,
                Title = a.Title,
                Description = a.Description,
                DateOfAssessment = a.DateOfAssessment,
                DifficultyLevel = a.DifficultyLevel,
                SubjectName = a.Subject?.Name ?? "Unknown",
                MemberNames = a.Subject?.Members?.Select(m => m.Name).ToList() ?? new List<string>(),
                BrainFoods = a.BrainFoods?.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    IngredientName = bf.Ingredient?.Name ?? "Unknown",
                    Quantity = bf.Quantity,
                    UnitPrice = bf.Ingredient?.UnitPrice ?? 0,
                    Benefits = bf.Ingredient?.Benefits ?? "",
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList();

            return new PaginatedResult<AssessmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }


    }
}
