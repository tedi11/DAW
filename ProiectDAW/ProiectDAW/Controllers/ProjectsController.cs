using Microsoft.AspNetCore.Mvc;

namespace ProiectDAW.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
