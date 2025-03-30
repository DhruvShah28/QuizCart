using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Controllers
{

    [Route("SubjectsPage")]
    public class SubjectsPageController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IAssessmentService _assessmentService;

        public SubjectsPageController(ISubjectService subjectService, IAssessmentService assessmentService)
        {
            _subjectService = subjectService;
            _assessmentService = assessmentService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var subjects = await _subjectService.ListSubjects();
            return View(subjects);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _subjectService.FindSubject(id);
            if (subject == null)
                return View("Error", new ErrorViewModel { Errors = ["Subject not found."] });

            var assessments = await _assessmentService.ListAssessmentsBySubjectId(id);

            var viewModel = new SubjectDetails
            {
                Subject = subject,
                Assessments = assessments?.ToList()
            };

            return View(viewModel);
        }

        [HttpGet("AddSubject")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("AddSubject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddSubjectDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _subjectService.AddSubject(dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }

        [HttpGet("EditSubject/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectService.FindSubject(id);
            if (subject == null)
                return View("Error", new ErrorViewModel { Errors = ["Subject not found."] });

            var dto = new UpdateSubjectDto
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                Description = subject.Description
            };

            return View(dto);
        }

        [HttpPost("EditSubject/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSubjectDto dto)
        {
            if (id != dto.SubjectId)
                return View("Error", new ErrorViewModel { Errors = ["ID mismatch."] });

            var result = await _subjectService.UpdateSubject(id, dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("DeleteSubject/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var subject = await _subjectService.FindSubject(id);
            if (subject == null)
                return View("Error", new ErrorViewModel { Errors = ["Subject not found."] });

            var assessments = await _assessmentService.ListAssessmentsBySubjectId(id);

            var viewModel = new SubjectDetails
            {
                Subject = subject,
                Assessments = assessments?.ToList()
            };

            return View("ConfirmDelete", viewModel);
        }

        [HttpPost("DeleteSubject/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _subjectService.DeleteSubject(id);

            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = result.Messages
                });
            }
        }



    }
}
