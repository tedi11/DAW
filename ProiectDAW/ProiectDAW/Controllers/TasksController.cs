using Microsoft.AspNetCore.Mvc;

namespace ProiectDAW.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
