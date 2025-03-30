using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuizCart.Controllers
{
    public class IngredientsPageController : Controller
    {
        // GET: IngredientsPageController

        public ActionResult Index()
        {
            return View();
        }

        // GET: IngredientsPageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: IngredientsPageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IngredientsPageController/Create
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

        // GET: IngredientsPageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IngredientsPageController/Edit/5
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

        // GET: IngredientsPageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IngredientsPageController/Delete/5
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
