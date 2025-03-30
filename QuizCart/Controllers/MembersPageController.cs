using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;
using QuizCart.Services;

namespace QuizCart.Controllers
{
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


        // Redirect to list
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MembersPage/ListMembers
        [HttpGet("ListMembers")]
        public async Task<IActionResult> List()
        {
            IEnumerable<MemberDto> members = await _memberService.ListMembers();
            return View(members);
        }

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



        // GET: MembersPage/AddMember
        [HttpGet("AddMember")]
        public IActionResult Add()
        {
            return View();
        }

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
