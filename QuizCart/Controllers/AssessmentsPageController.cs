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

        /// <summary>
        /// Redirects to the List view of assessments.
        /// </summary>
        /// <returns>Redirect to List action</returns>


        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays a list of all assessments.
        /// </summary>
        /// <returns>View with list of assessments</returns>


        [HttpGet("List")]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var result = await _assessmentService.GetPaginatedAssessments(page, pageSize);
            return View(result);
        }


        /// <summary>
        /// Displays detailed information of a specific assessment.
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Assessment detail view or error if not found</returns>


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

        /// <summary>
        /// Displays the form to add a new assessment.
        /// </summary>
        /// <returns>Add assessment view</returns>


        [HttpGet("AddAssessment")]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Adds a new assessment to the system.
        /// </summary>
        /// <param name="dto">Assessment details</param>
        /// <returns>Redirects to List view if successful; error view otherwise</returns>


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

        /// <summary>
        /// Displays the form to edit an existing assessment.
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Edit view with pre-filled data or error view if not found</returns>


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

        /// <summary>
        /// Updates an existing assessment.
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <param name="dto">Updated assessment data</param>
        /// <returns>Redirect to details view if successful; error view otherwise</returns>

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

        /// <summary>
        /// Displays confirmation page to delete an assessment.
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Delete confirmation view or error if not found</returns>

        [HttpGet("DeleteAssessment/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var assessment = await _assessmentService.FindAssessment(id);
            if (assessment == null)
                return View("Error", new ErrorViewModel { Errors = ["Assessment not found."] });

            return View(assessment);
        }

        /// <summary>
        /// Deletes a specific assessment.
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Redirect to list if deleted successfully; error view otherwise</returns>


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
