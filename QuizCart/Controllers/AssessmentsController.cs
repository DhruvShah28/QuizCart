using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        /// <summary>
        /// Retrieves a list of all assessments with related subject and brain food details.
        /// </summary>
        /// <returns>
        /// HTTP 200 OK with a list of AssessmentDto objects.
        /// </returns>
        /// <example>
        /// GET: api/Assessments/List
        /// Response:
        /// [
        ///   {
        ///     "assessmentId": 1,
        ///     "title": "Math Basics",
        ///     "description": "Covers addition, subtraction, multiplication",
        ///     "dateOfAssessment": "2025-04-01",
        ///     "difficultyLevel": "Easy",
        ///     "subjectName": "Mathematics",
        ///     "memberNames": ["Alice", "Bob"],
        ///     "brainFoods": [
        ///       {
        ///         "brainFoodId": 5,
        ///         "ingredientId": 2,
        ///         "assessmentId": 1,
        ///         "quantity": 3,
        ///         "assessmentName": "Math Basics",
        ///         "ingredientName": "Almonds",
        ///         "benefits": "Improves memory",
        ///         "unitPrice": 1.5,
        ///         "purchases": []
        ///       }
        ///     ]
        ///   }
        /// ]
        /// </example>


        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> ListAssessments()
        {
            var result = await _assessmentService.ListAssessments();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves an assessment by its ID.
        /// </summary>
        /// <param name="id">ID of the assessment to retrieve.</param>
        /// <returns>
        /// HTTP 200 OK with AssessmentDto if found, 404 Not Found otherwise.
        /// </returns>
        /// <example>
        /// GET: api/Assessments/Find/1
        /// Response:
        /// {
        ///   "assessmentId": 1,
        ///   "title": "Math Basics",
        ///   "description": "Covers addition, subtraction, multiplication",
        ///   "dateOfAssessment": "2025-04-01",
        ///   "difficultyLevel": "Easy",
        ///   "subjectName": "Mathematics",
        ///   "memberNames": ["Alice", "Bob"],
        ///    "brainFoods": [
        ///       {
        ///         "brainFoodId": 5,
        ///         "ingredientId": 2,
        ///         "assessmentId": 1,
        ///         "quantity": 3,
        ///         "assessmentName": "Math Basics",
        ///         "ingredientName": "Almonds",
        ///         "benefits": "Improves memory",
        ///         "unitPrice": 1.5,
        ///         "purchases": []
        ///       }
        /// }
        /// </example>


        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AssessmentDto>> FindAssessment(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);

            if (assessment == null)
                return NotFound($"Assessment with ID {id} not found.");

            return Ok(assessment);
        }

        /// <summary>
        /// Adds a new assessment.
        /// </summary>
        /// <param name="dto">The AddAssessmentDto containing assessment details.</param>
        /// <returns>
        /// HTTP 201 Created if successful, 500 Internal Server Error if error occurs.
        /// </returns>
        /// <example>
        /// POST: api/Assessments/Add
        /// Request Body:
        /// {
        ///   "title": "Science Basics",
        ///   "description": "Covers physics and chemistry",
        ///   "dateOfAssessment": "2025-04-02",
        ///   "difficultyLevel": "Medium",
        ///   "subjectId": 2
        /// }
        /// </example>


        [HttpPost("Add")]
        public async Task<IActionResult> AddAssessment(AddAssessmentDto dto)
        {
            var response = await _assessmentService.AddAssessment(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Error adding assessment." });

            return CreatedAtAction(nameof(FindAssessment), new { id = response.CreatedId }, new
            {
                message = $"Assessment added successfully with ID {response.CreatedId}",
                assessmentId = response.CreatedId
            });
        }

        /// <summary>
        /// Updates an existing assessment.
        /// </summary>
        /// <param name="id">The ID of the assessment to update.</param>
        /// <param name="dto">The updated assessment details.</param>
        /// <returns>
        /// HTTP 200 OK if successful, 400 Bad Request for ID mismatch, 404 Not Found or 500 Internal Server Error.
        /// </returns>
        /// <example>
        /// PUT: api/Assessments/Update/1
        /// Request Body:
        /// {
        ///   "assessmentId": 1,
        ///   "title": "Updated Science Basics",
        ///   "description": "Updated description",
        ///   "dateOfAssessment": "2025-05-01",
        ///   "difficultyLevel": "Hard"
        /// }
        /// </example>


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAssessment(int id, UpdateAssessmentDto dto)
        {
            if (id != dto.AssessmentId)
                return BadRequest(new { message = "Assessment ID mismatch." });

            var response = await _assessmentService.UpdateAssessment(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Assessment not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error updating assessment." });

            return Ok(new { message = $"Assessment with ID {id} updated successfully." });
        }

        /// <summary>
        /// Deletes an assessment by ID.
        /// </summary>
        /// <param name="id">The ID of the assessment to delete.</param>
        /// <returns>
        /// HTTP 200 OK if deleted, 404 Not Found if not found, 500 Internal Server Error otherwise.
        /// </returns>
        /// <example>
        /// DELETE: api/Assessments/Delete/1
        /// </example>


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var response = await _assessmentService.DeleteAssessment(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Assessment not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error deleting assessment." });

            return Ok(new { message = $"Assessment with ID {id} deleted successfully." });
        }

        /// <summary>
        /// Retrieves all assessments for a specific subject ID.
        /// </summary>
        /// <param name="subjectId">The ID of the subject.</param>
        /// <returns>
        /// HTTP 200 OK with list of AssessmentDto, or 404 if no assessments found.
        /// </returns>
        /// <example>
        /// GET: api/Assessments/Assessments/2
        /// Response:
        /// [
        ///   {
        ///     "assessmentId": 4,
        ///     "title": "Physics Quiz",
        ///     "description": "Newton's laws and thermodynamics",
        ///     "dateOfAssessment": "2025-04-15",
        ///     "difficultyLevel": "Medium",
        ///     "subjectName": "Science",
        ///     "memberNames": ["Charlie"],
        ///     "brainFoods": [
        ///       {
        ///         "brainFoodId": 10,
        ///         "ingredientId": 3,
        ///         "assessmentId": 4,
        ///         "quantity": 2,
        ///         "assessmentName": "Physics Quiz",
        ///         "ingredientName": "Blueberries",
        ///         "benefits": "Boosts cognitive function",
        ///         "unitPrice": 1.75,
        ///         "purchases": [
        ///           {
        ///             "memberName": "Charlie",
        ///             "datePurchased": "2025-04-10"
        ///           }
        ///         ]
        ///       }
        ///     ]
        ///   }
        /// ]
        /// </example>


        [HttpGet("Assessments/{subjectId}")]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> ListAssessmentsBySubjectId(int subjectId)
        {
            var assessments = await _assessmentService.ListAssessmentsBySubjectId(subjectId);

            if (!assessments.Any())
            {
                return NotFound(new { message = $"No assessments found for subject ID {subjectId}" });
            }

            return Ok(assessments);
        }

    }
}
