using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;
using QuizCart.Services;

namespace QuizCart.Controllers
{
    /// <summary>
    /// Controller for managing member-related web views including listing, viewing details, adding, editing, deleting, and linking subjects.
    /// </summary>


    [Route("MembersPage")]
    public class MembersPageController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ISubjectService _subjectService;
        private readonly IPurchaseService _purchaseService;

        public MembersPageController(
            IMemberService memberService,
            ISubjectService subjectService,
            IPurchaseService purchaseService)
        {
            _memberService = memberService;
            _subjectService = subjectService;
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Redirects base MembersPage route to list action.
        /// </summary>


        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Lists all members with financial and participation summaries.
        /// </summary>
        /// <returns>View with a list of MemberDto</returns>


        // GET: MembersPage/ListMembers
        [HttpGet("List")]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var result = await _memberService.GetPaginatedMembers(page, pageSize);
            return View(result);
        }


        /// <summary>
        /// Displays details of a specific member including linked subjects and purchases.
        /// </summary>
        /// <param name="id">The ID of the member.</param>
        /// <returns>Detailed view of the member or error view.</returns>


        [HttpGet("MemberDetails/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberService.FindMember(id);
            if (member == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Member not found."] });
            }

            var purchases = await _purchaseService.ListPurchasesByMemberId(id);

            var allSubjects = await _subjectService.ListSubjects();

            var linkedSubjects = await _subjectService.ListSubjectsByMemberId(id);

            var unlinkedSubjects = allSubjects
                .Where(s => linkedSubjects.All(ls => ls.SubjectId != s.SubjectId))
                .ToList();

            var viewModel = new MemberDetails
            {
                Member = member,
                Subjects = linkedSubjects?.ToList(),
                Purchases = purchases?.ToList(),
                UnlinkedSubjects = unlinkedSubjects 
            };

            return View(viewModel);
        }

        /// <summary>
        /// Displays the form for adding a new member.
        /// </summary>


        // GET: MembersPage/AddMember
        [HttpGet("AddMember")]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Submits a new member record.
        /// </summary>
        /// <param name="dto">Member data to add.</param>
        /// <returns>Redirect to list or error view.</returns>


        // POST: MembersPage/AddMember
        [HttpPost("AddMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMemberDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _memberService.AddMember(dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays the form for editing an existing member.
        /// </summary>
        /// <param name="id">ID of the member to edit.</param>
        /// <returns>Pre-filled form view or error if not found.</returns>


        // GET: MembersPage/EditMember/{id}
        [HttpGet("EditMember/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _memberService.FindMember(id);

            if (member == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Member not found."] });
            }

            var dto = new UpdateMemberDto
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email
            };

            return View(dto);
        }

        /// <summary>
        /// Submits updated data for an existing member.
        /// </summary>
        /// <param name="id">ID of the member being edited.</param>
        /// <param name="dto">Updated member data.</param>
        /// <returns>Redirect to details or error view.</returns>


        // POST: MembersPage/EditMember/{id}
        [HttpPost("EditMember/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdateMemberDto dto)
        {
            if (id != dto.MemberId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Member ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _memberService.UpdateMember(id, dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Displays confirmation for deleting a member.
        /// </summary>
        /// <param name="id">ID of the member to delete.</param>
        /// <returns>Confirmation view or error view.</returns>


        // GET: MembersPage/DeleteMember/{id}
        [HttpGet("DeleteMember/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var member = await _memberService.FindMember(id);

            if (member == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Member not found."] });
            }

            return View(member);
        }

        /// <summary>
        /// Deletes a member after confirmation.
        /// </summary>
        /// <param name="id">ID of the member.</param>
        /// <returns>Redirect to list or error view.</returns>


        // POST: MembersPage/DeleteMember/{id}
        [HttpPost("DeleteMember/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _memberService.DeleteMember(id);

            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }
        }

        /// <summary>
        /// Links a subject to a member.
        /// </summary>
        /// <param name="dto">MemberId and SubjectId to link.</param>
        /// <returns>Redirect to member details or error view.</returns>


        [HttpPost("LinkSubject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkSubject(LinkSubjectDto dto)
        {
            var response = await _memberService.LinkSubject(dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }

            return RedirectToAction("Details", new { id = dto.MemberId });
        }

        /// <summary>
        /// Unlinks a subject from a member.
        /// </summary>
        /// <param name="dto">MemberId and SubjectId to unlink.</param>
        /// <returns>Redirect to member details or error view.</returns>


        [HttpPost("UnlinkSubject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkSubject(LinkSubjectDto dto)
        {
            var response = await _memberService.UnlinkSubject(dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }

            return RedirectToAction("Details", new { id = dto.MemberId });
        }


    }
}
