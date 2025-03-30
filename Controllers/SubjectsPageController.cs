using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuizCart.Controllers
{
    public class SubjectsPageController : Controller
    {
        // GET: SubjectsPageController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SubjectsPageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SubjectsPageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubjectsPageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubjectsPageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SubjectsPageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubjectsPageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SubjectsPageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
