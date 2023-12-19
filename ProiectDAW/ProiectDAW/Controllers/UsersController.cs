using Microsoft.AspNetCore.Mvc;

namespace ProiectDAW.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
