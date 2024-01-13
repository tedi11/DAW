using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.Data;
using ProiectDAW.Models;

namespace ProiectDAW.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
            private readonly ApplicationDbContext db;
            private readonly UserManager<AppUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            public ProjectsController(
                ApplicationDbContext context,
                UserManager<AppUser> userManager,
                RoleManager<IdentityRole> roleManager
            )
            {
                db = context;
                _userManager = userManager;
                _roleManager = roleManager;
            }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(User);
            var projs = db.Members.Where(userpr => userpr.UserId == userid).Select(c => c.ProjectId);
            List<int?> proj_ids = projs.ToList();

            var projects = db.Projects.Include("User").Where(proj => proj_ids.Contains(proj.Id)).OrderBy(c => c.Name);

            Console.WriteLine(projects);

            string[] icons = { "bi-bounding-box", "bi-brightness-high-fill", "bi-bell", "bi-bookmarks-fill", "bi-basket3-fill", "bi-arrow-down-up", "bi-award" };

            ViewBag.Icons = icons;


            if (!projects.Any())
            {
                ViewBag.Projects = 0;
            }
            else
            {
                ViewBag.Projects = projects;
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult IndexAdmin()
        {

            var projects = db.Projects.Include("User");
            ViewBag.Projects = projects;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Project project = new Project();
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> NewAsync(Project project)
        {
            var user_id = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                project.ManagerId = user_id;
                Console.WriteLine(project.ManagerId);

                /*project.ManagerEmail = db.AppUsers
                    .Include("Project") 
                    .Where(utilizator => utilizator.Id == user_id)
                    .Select(utilizator => utilizator.Email) 
                    .FirstOrDefault();*/

                Member project_data = new Member();
                project_data.UserId = user_id;
                project_data.ProjectId = project.Id;
                project_data.Project = project;

                db.Members.Add(project_data);
                await db.SaveChangesAsync();
                TempData["message"] = "Proiectul a fost adăugat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(project);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            var userid = _userManager.GetUserId(User);
            var users = db.Members.Where(userpr => userpr.ProjectId == id).Select(c => c.UserId);
            List<string?> user_ids = users.ToList();
            var project = db.Projects.Include("Tasks.User").Include("User").Where(proj => proj.Id == id).First();
            
            if (user_ids.Contains(userid) || User.IsInRole("Admin"))
            {
                ViewBag.Users = db.Members.Include("User").Where(user => user.ProjectId == id);
                return View(project);
            }
            else
            {
                TempData["message"] = "Eroare! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Models.Task task)
        {
            bool flag = true;
            if (DateTime.Compare(task.StartDate, task.Deadline) >= 0)
            {
                TempData["messageerr"] = "Data de inceput trebuie sa fie inainte de deadline!";
                flag = false;
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            if (task.Title == null || task.Description == null)
            {
                flag = false;
            }
            if (ModelState.IsValid && flag)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + task.Id);
            }
            else
            {
                Project project = db.Projects.Include("Tasks").Include("User").Where(proj => proj.Id == task.ProjectId).First();
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var userid = _userManager.GetUserId(User);
            Project project = db.Projects.Include("Tasks")
                                        .Where(proj => proj.Id == id)
                                        .First();
            if (project.ManagerId == userid || User.IsInRole("Admin"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Show/" + id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Project requestProject)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                TempData["message"] = "Eroare BD!";
                return View(requestProject);

            }
            else
            {
                var userid = _userManager.GetUserId(User);
                if (project.ManagerId == userid || User.IsInRole("Admin"))
                {
                    if (ModelState.IsValid)
                    {
                        project.Name = requestProject.Name;
                        TempData["message"] = "Proiectul a fost modificat";
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(requestProject);
                    }
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces";
                    ViewBag.Message = TempData["message"];
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Include("Tasks.Comments").Where(proj => proj.Id == id).First();
            if (project == null)
            {
                TempData["message"] = "Eroare BD!";
            }
            else
            {
                var userid = _userManager.GetUserId(User);
                if (project.ManagerId == userid || User.IsInRole("Admin"))
                {
                    db.Projects.Remove(project);
                    db.SaveChanges();
                    TempData["message"] = "Proiectul a fost sters!";
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces";
                    ViewBag.Message = TempData["message"];
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Users(int id)
        {
            var userid = _userManager.GetUserId(User);
            var users = db.Members.Include("User").Where(user => user.ProjectId == id);
            List<string?> users_ids = users.Select(c => c.UserId).ToList();
            if (users_ids.Contains(userid) || User.IsInRole("Admin"))
            {

                var project = db.Projects.Find(id);
                ViewBag.Users = users;
                ViewBag.Project = project;
                ViewBag.AllUsers = db.Users;
                
                return View();
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Users(int id, [FromForm] Member requestUser)
        {
            var userid = _userManager.GetUserId(User);
            var project = db.Projects.Find(id);
            if (project == null)
            {
                TempData["message"] = "Eroare BD!";
                return RedirectToAction("Index");
            }
            else
            {
                if (userid == project.ManagerId || User.IsInRole("Admin"))
                {
                    Member userProject = new Member();
                    userProject.ProjectId = id;
                    userProject.UserId = requestUser.UserId;
                    db.Members.Add(userProject);
                    db.SaveChanges();
                    return Redirect("/Projects/Users/" + project.Id);
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces";
                    ViewBag.Message = TempData["message"];
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult UserDelete(int id, string rmvuser, int rmvproject)
        {
            var userid = _userManager.GetUserId(User);
            var users = db.Members.Include("User").Where(user => user.ProjectId == rmvproject);
            List<string?> users_ids = users.Select(c => c.UserId).ToList();
            if ((users_ids.Contains(userid) && (rmvuser == userid || users_ids.Contains(userid)))
                    || User.IsInRole("Admin"))
            {
                Member user_project = db.Members.Find(id, rmvuser, rmvproject);
                var project_id = user_project.ProjectId;
                if (user_project == null)
                {
                    TempData["message"] = "Eroare BD!";
                    return Redirect("/Projects/Users/" + project_id);
                }
                else
                {
                    db.Members.Remove(user_project);
                    db.SaveChanges();
                    if (user_project.UserId == _userManager.GetUserId(User))
                    {
                        TempData["message"] = "Ai părăsit proiectul!";
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        TempData["message"] = "Utilizator scos.";
                        return Redirect("/Projects/Users/" + project_id);
                    }
                }
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("Index");
            }
        }
    }
}
