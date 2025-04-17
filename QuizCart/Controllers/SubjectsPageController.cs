using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Controllers
{
    /// <summary>
    /// Controller for managing purchase records through web pages.
    /// Handles listing, adding, editing, viewing, and deleting purchases.
    /// </summary>

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

        /// <summary>
        /// Redirects base route to List.
        /// </summary>


        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays a list of all subjects.
        /// </summary>
        /// <returns>A view containing a list of subjects.</returns>

        [HttpGet("List")]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var result = await _subjectService.GetPaginatedSubjects(page, pageSize);
            return View(result);
        }


        /// <summary>
        /// Displays details of a specific subject and its associated assessments.
        /// </summary>
        /// <param name="id">ID of the subject.</param>
        /// <returns>A view showing subject details and assessments, or an error if not found.</returns>


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

        /// <summary>
        /// Displays the form to add a new subject.
        /// </summary>


        [HttpGet("AddSubject")]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of a new subject.
        /// </summary>
        /// <param name="dto">The subject data to add.</param>
        /// <returns>Redirects to list if successful, or error view otherwise.</returns>


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

        /// <summary>
        /// Displays the form to edit a subject.
        /// </summary>
        /// <param name="id">The ID of the subject to edit.</param>
        /// <returns>The edit form or an error view.</returns>


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

        /// <summary>
        /// Handles the submission of an edited subject.
        /// </summary>
        /// <param name="id">The ID of the subject to update.</param>
        /// <param name="dto">Updated subject data.</param>
        /// <returns>Redirects to details view if successful, or error view otherwise.</returns>



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

        /// <summary>
        /// Displays a confirmation view for subject deletion.
        /// </summary>
        /// <param name="id">The ID of the subject to delete.</param>
        /// <returns>Confirmation view or error if not found.</returns>


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

        /// <summary>
        /// Handles the deletion of a subject.
        /// </summary>
        /// <param name="id">The ID of the subject to delete.</param>
        /// <returns>Redirects to list view if successful, or error view otherwise.</returns>


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
