using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuizCart.Controllers
{
    public class MembersPageController : Controller
    {
        // GET: MembersPageController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MembersPageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MembersPageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembersPageController/Create
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

        // GET: MembersPageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MembersPageController/Edit/5
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

        // GET: MembersPageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MembersPageController/Delete/5
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
