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

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> ListAssessments()
        {
            var result = await _assessmentService.ListAssessments();
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AssessmentDto>> FindAssessment(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);

            if (assessment == null)
                return NotFound($"Assessment with ID {id} not found.");

            return Ok(assessment);
        }

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
