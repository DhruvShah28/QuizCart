using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> ListSubjects()
        {
            var result = await _subjectService.ListSubjects();
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<SubjectDto>> FindSubject(int id)
        {
            var subject = await _subjectService.FindSubject(id);

            if (subject == null)
                return NotFound($"Subject with ID {id} not found.");

            return Ok(subject);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSubject(AddSubjectDto dto)
        {
            var response = await _subjectService.AddSubject(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Error adding subject." });

            return CreatedAtAction(nameof(FindSubject), new { id = response.CreatedId }, new
            {
                message = $"Subject added successfully with ID {response.CreatedId}",
                subjectId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto dto)
        {
            if (id != dto.SubjectId)
                return BadRequest(new { message = "Subject ID mismatch." });

            var response = await _subjectService.UpdateSubject(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Subject not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error updating subject." });

            return Ok(new { message = $"Subject with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var response = await _subjectService.DeleteSubject(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Subject not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error deleting subject." });

            return Ok(new { message = $"Subject with ID {id} deleted successfully." });
        }
    }
}
