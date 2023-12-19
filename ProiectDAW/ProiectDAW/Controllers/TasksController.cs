using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectDAW.Data;
using ProiectDAW.Models;

namespace ProiectDAW.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TasksController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
