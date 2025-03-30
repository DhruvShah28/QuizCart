using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuizCart.Controllers
{
    public class BrainFoodsPageController : Controller
    {
        // GET: BrainFoodsPageController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BrainFoodsPageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BrainFoodsPageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BrainFoodsPageController/Create
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

        // GET: BrainFoodsPageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BrainFoodsPageController/Edit/5
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

        // GET: BrainFoodsPageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BrainFoodsPageController/Delete/5
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
