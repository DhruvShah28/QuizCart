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

        /// <summary>
        /// Returns a list of all subjects along with related metadata.
        /// </summary>
        /// <returns>
        /// HTTP 200 OK with a list of SubjectDto objects.
        /// </returns>
        /// <example>
        /// GET: api/Subjects/List -> [ { SubjectId: 1, Name: "Math" }, ... ]
        /// </example>


        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> ListSubjects()
        {
            var result = await _subjectService.ListSubjects();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a subject by its ID.
        /// </summary>
        /// <param name="id">ID of the subject to retrieve.</param>
        /// <returns>
        /// HTTP 200 OK with the subject data, or 404 Not Found if not found.
        /// </returns>
        /// <example>
        /// GET: api/Subjects/Find/1 -> { SubjectId: 1, Name: "Science" }
        /// </example>


        [HttpGet("Find/{id}")]
        public async Task<ActionResult<SubjectDto>> FindSubject(int id)
        {
            var subject = await _subjectService.FindSubject(id);

            if (subject == null)
                return NotFound($"Subject with ID {id} not found.");

            return Ok(subject);
        }

        /// <summary>
        /// Adds a new subject to the system.
        /// </summary>
        /// <param name="dto">The data to create a new subject.</param>
        /// <returns>
        /// HTTP 201 Created with the subject ID, or 500 Internal Server Error if an error occurs.
        /// </returns>
        /// <example>
        /// POST: api/Subjects/Add -> { Name: "History" }
        /// </example>


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

        /// <summary>
        /// Updates an existing subject by ID.
        /// </summary>
        /// <param name="id">ID of the subject to update.</param>
        /// <param name="dto">The updated data for the subject.</param>
        /// <returns>
        /// HTTP 200 OK if successful, 400 Bad Request for ID mismatch, 404 Not Found or 500 Internal Server Error.
        /// </returns>
        /// <example>
        /// PUT: api/Subjects/Update/1 -> { SubjectId: 1, Name: "Advanced Math" }
        /// </example>


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

        /// <summary>
        /// Deletes a subject by ID.
        /// </summary>
        /// <param name="id">ID of the subject to delete.</param>
        /// <returns>
        /// HTTP 200 OK if deleted, 404 Not Found if subject does not exist, 500 Internal Server Error on failure.
        /// </returns>
        /// <example>
        /// DELETE: api/Subjects/Delete/1
        /// </example>


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
