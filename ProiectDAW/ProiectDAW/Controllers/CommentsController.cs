using Microsoft.AspNetCore.Mvc;

namespace ProiectDAW.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
