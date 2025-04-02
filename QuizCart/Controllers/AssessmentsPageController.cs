using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Controllers
{
    [Route("AssessmentsPage")]
    public class AssessmentsPageController : Controller
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsPageController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var assessments = await _assessmentService.ListAssessments();
            return View(assessments);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);
            if (assessment == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Assessment not found."] });
            }

            return View(assessment);
        }



        [HttpGet("AddAssessment")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("AddAssessment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAssessmentDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _assessmentService.AddAssessment(dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }

        [HttpGet("EditAssessment/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);
            if (assessment == null)
                return View("Error", new ErrorViewModel { Errors = ["Assessment not found."] });

            var dto = new UpdateAssessmentDto
            {
                AssessmentId = assessment.AssessmentId,
                Title = assessment.Title,
                Description = assessment.Description,
                DateOfAssessment = assessment.DateOfAssessment,
                DifficultyLevel = assessment.DifficultyLevel
            };

            return View(dto);
        }

        [HttpPost("EditAssessment/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAssessmentDto dto)
        {
            if (id != dto.AssessmentId)
                return View("Error", new ErrorViewModel { Errors = ["Assessment ID mismatch."] });

            var result = await _assessmentService.UpdateAssessment(id, dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("DeleteAssessment/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);
            if (assessment == null)
                return View("Error", new ErrorViewModel { Errors = ["Assessment not found."] });

            return View(assessment);
        }

        [HttpPost("DeleteAssessment/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assessmentService.DeleteAssessment(id);
            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
                return RedirectToAction("List");

            return View("Error", new ErrorViewModel { Errors = result.Messages });
        }
    }
}
