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

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> ListMembers()
        {
            IEnumerable<MemberDto> MemberDtos = await _memberService.ListMembers();
            return Ok(MemberDtos);
        }

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



        [HttpPost("LinkSubject")]
        public async Task<IActionResult> LinkSubject([FromBody] LinkSubjectDto dto)
        {
            var result = await _memberService.LinkSubject(dto);
            if (result.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(result.Messages);
            return Ok(new { message = "Subject linked to member successfully." });
        }

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
