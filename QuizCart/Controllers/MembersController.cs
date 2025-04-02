using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Services;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Returns a list of all members with details like name, email, amount owed, and amount paid.
        /// </summary>
        /// <returns>
        /// HTTP 200 OK with a list of MemberDto.
        /// </returns>
        /// <example>
        /// GET: api/Members/List -> [{ MemberId: 1, Name: "Vicky", Email: "vicky@example.com", ... }, ...]
        /// </example>

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> ListMembers()
        {
            IEnumerable<MemberDto> MemberDtos = await _memberService.ListMembers();
            return Ok(MemberDtos);
        }

        /// <summary>
        /// Retrieves a specific member by ID.
        /// </summary>
        /// <param name="id">The ID of the member.</param>
        /// <returns>
        /// HTTP 200 OK with MemberDto if found, otherwise 404 Not Found.
        /// </returns>
        /// <example>
        /// GET: api/Members/Find/1 -> { MemberId: 1, Name: "Vicky", Email: "vicky@example.com", ... }
        /// </example>

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<MemberDto>> FindMember(int id)
        {
            var member = await _memberService.FindMember(id);

            if (member == null)
            {
                return NotFound($"Member with ID {id} doesn't exist");
            }
            else
            {
                return Ok(member);
            }
        }

        /// <summary>
        /// Adds a new member.
        /// </summary>
        /// <param name="dto">AddMemberDto containing name and email.</param>
        /// <returns>
        /// HTTP 201 Created with new member ID or 500 Internal Server Error if failed.
        /// </returns>
        /// <example>
        /// POST: api/Members/Add -> { Name: "Vicky", Email: "vicky@example.com" }
        /// </example>

        [HttpPost("Add")]
        public async Task<IActionResult> AddMember(AddMemberDto dto)
        {

            ServiceResponse response = await _memberService.AddMember(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while adding the member." });
            }

            return CreatedAtAction("FindMember", new { id = response.CreatedId }, new
            {
                message = $"Member added successfully with ID {response.CreatedId}",
                memberId = response.CreatedId
            });
        }

        /// <summary>
        /// Updates an existing member.
        /// </summary>
        /// <param name="id">The ID of the member to update.</param>
        /// <param name="dto">UpdateMemberDto containing updated details.</param>
        /// <returns>
        /// HTTP 200 OK if successful, 400 for mismatch, 404 if not found, or 500 if error.
        /// </returns>
        /// <example>
        /// PUT: api/Members/Update/1 -> { MemberId: 1, Name: "Updated Name", Email: "updated@example.com" }
        /// </example>


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMember(int id, UpdateMemberDto dto)
        {
            if (id != dto.MemberId)
            {
                return BadRequest(new { message = "Member ID mismatch." });
            }
            ServiceResponse response = await _memberService.UpdateMember(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Member not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while updating the member." });
            }

            return Ok(new { message = $"Member with ID {id} updated successfully." });
        }

        /// <summary>
        /// Deletes a member by ID.
        /// </summary>
        /// <param name="id">The ID of the member to delete.</param>
        /// <returns>
        /// HTTP 200 OK if deleted, 404 if not found, 500 if error.
        /// </returns>
        /// <example>
        /// DELETE: api/Members/Delete/1
        /// </example>


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            ServiceResponse response = await _memberService.DeleteMember(id);


            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Member not found." });
            }

            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the member." });
            }


            return Ok(new { message = $"Member with ID {id} deleted successfully." });
        }

        /// <summary>
        /// Links a subject to a member.
        /// </summary>
        /// <param name="dto">LinkSubjectDto containing MemberId and SubjectId.</param>
        /// <returns>
        /// HTTP 200 OK if linked, 404 if not found.
        /// </returns>
        /// <example>
        /// POST: api/Members/LinkSubject -> { MemberId: 1, SubjectId: 2 }
        /// </example>


        [HttpPost("LinkSubject")]
        public async Task<IActionResult> LinkSubject([FromBody] LinkSubjectDto dto)
        {
            var result = await _memberService.LinkSubject(dto);
            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(result.Messages);
            return Ok(new { message = "Subject linked to member successfully." });
        }

        /// <summary>
        /// Unlinks a subject from a member.
        /// </summary>
        /// <param name="dto">LinkSubjectDto containing MemberId and SubjectId.</param>
        /// <returns>
        /// HTTP 200 OK if unlinked, 404 if not found.
        /// </returns>
        /// <example>
        /// DELETE: api/Members/UnlinkSubject -> { MemberId: 1, SubjectId: 2 }
        /// </example>


        [HttpDelete("UnlinkSubject")]
        public async Task<IActionResult> UnlinkSubject([FromBody] LinkSubjectDto dto)
        {
            var result = await _memberService.UnlinkSubject(dto);
            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(result.Messages);
            return Ok(new { message = "Subject unlinked from member successfully." });
        }

    }
}
