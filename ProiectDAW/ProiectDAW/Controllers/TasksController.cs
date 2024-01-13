using ProiectDAW.Data;
using Microsoft.AspNetCore.Mvc;
using ProiectDAW.Models;
using Task = ProiectDAW.Models.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ProiectDAW.Controllers
{
    [Authorize(Roles = "User,Admin")]
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

        [NonAction]
        public bool CheckUser(int? proj_id) ///verific daca userul e pe proiect
        {
            if (proj_id == null)
                return false;
            var userid = _userManager.GetUserId(User);
            var users = db.Members.Where(userpr => userpr.ProjectId == proj_id).Select(c => c.UserId);
            List<string?> user_ids = users.ToList();
            if (user_ids.Contains(userid))
                return true;
            return false;
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllStatuses()
        {
            var statuses = new List<SelectListItem>();
            string[] all_statuses = { "not started",
                                      "in progress",
                                      "completed" };
            foreach (var status in all_statuses)
            {
                string aux;
                if (status.Length == 1)
                {
                    aux = status.ToUpper();
                }
                else
                {
                    aux = char.ToUpper(status[0]) + status[1..];
                }

                statuses.Add(new SelectListItem
                {
                    Value = aux,
                    Text = aux,

                });



            }
            return statuses;
        }
        public IActionResult Show(int id)
        {

            var task = db.Tasks.Include("Comments.User").Include("User").Include("Project")
                                .Where(tsk => tsk.Id == id).First();
            ViewBag.Users = db.Members.Include("User")
                .Where(c => c.ProjectId == task.ProjectId).OrderBy(c => c.User.UserName);

            if (CheckUser(task.ProjectId) || User.IsInRole("Admin"))
            {
                task.Statuses = GetAllStatuses();
                ViewBag.CurrentStatus = task.Status;
                Console.WriteLine(task.Status);
                Console.WriteLine("------------------------------");
                Console.WriteLine(task.Statuses);
                return View(task);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            Task task = db.Tasks.Include("Comments.User").Include("User").Include("Project").Where(tsk => tsk.Id == comment.TaskId).First();
            if (CheckUser(task.ProjectId) || User.IsInRole("Admin"))
            {
                comment.UserId = _userManager.GetUserId(User);

                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    return Redirect("/Tasks/Show/" + comment.TaskId);

                }
                else
                {
                    ViewBag.Users = db.Members.Include("User")
                        .Where(c => c.ProjectId == task.ProjectId).OrderBy(c => c.User.UserName);
                    task.Statuses = GetAllStatuses();
                    ViewBag.CurrentStatus = task.Status;
                    return View(task);
                }

            }
            else
            {
                TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }

        [HttpPost]
        public IActionResult AddUser([FromForm] int TaskId, [FromForm] string userId) ///sa pun un user task
        {
            var userid = _userManager.GetUserId(User);
            Task taskaux = db.Tasks.Include("Project").Where(a => a.Id == TaskId).First();
            if (userid == taskaux.Project.ManagerId || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid && (userId != "Add a user for this task"))
                {
                    if (db.Tasks
                        .Where(task => task.Id == TaskId && task.UserId == userId)
                        .Count() > 0)
                    {
                        TempData["message"] = "Utilizatorul deja are acest task";
                        TempData["messageType"] = "alert-danger";
                    }
                    else
                    {
                        Task task = db.Tasks.Find(TaskId);
                        if (task != null)
                        {
                            task.UserId = userId;
                            db.SaveChanges();

                            TempData["message"] = "Utilizator adaugat la task";
                            TempData["messageType"] = "alert-success";
                        }
                        else
                        {
                            TempData["message"] = "Eroare BD!";
                            TempData["messageType"] = "alert-danger";
                        }
                    }

                }
                else
                {
                    TempData["message"] = "Try again!";
                    TempData["messageType"] = "alert-danger";
                }

                return Redirect("/Tasks/Show/" + TaskId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }

        [HttpPost]
        public IActionResult ChangeStatus([FromForm] int TaskId, [FromForm] string newStatus)
        {
            var userid = _userManager.GetUserId(User);
            Task taskaux = db.Tasks.Where(a => a.Id == TaskId).First();
            if (CheckUser(taskaux.ProjectId) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    Task task = db.Tasks.Find(TaskId);
                    if (task != null)
                    {
                        task.Status = newStatus;
                        db.SaveChanges();

                        TempData["message"] = "Status schimbat";
                        TempData["messageType"] = "alert-success";
                    }
                    else
                    {
                        TempData["message"] = "Eroare BD!";
                        TempData["messageType"] = "alert-danger";
                    }
                }
                else
                {
                    TempData["message"] = "Try again!";
                    TempData["messageType"] = "alert-danger";
                }

                return Redirect("/Tasks/Show/" + TaskId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
        public IActionResult Edit(int id)
        {

            Task task = db.Tasks.Include("Comments").Include("Project")
                                        .Where(tsk => tsk.Id == id)
                                        .First();
            if (task == null)
            {
                TempData["message"] = "Eroare BD!";
                ViewBag.Message = TempData["message"];
                return View("/Projects/Index");
            }
            else
            {
                if (task.Project.ManagerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    if (TempData.ContainsKey("message"))
                    {
                        ViewBag.Message = TempData["message"];
                    }
                    ViewBag.CurrentStatus = task.Status;
                    return View(task);
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                    ViewBag.Message = TempData["message"];
                    return Redirect("/Projects/Index");
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Task requestTask)
        {
            Task task = db.Tasks.Find(id);

            if (task == null)
            {
                TempData["message"] = "Eroare BD!";
                ViewBag.Message = TempData["message"];
                return View(requestTask);

            }
            else
            {
                Task task_aux = db.Tasks.Include("Project").Where(c => c.Id == id).First();

                if (task_aux.Project.ManagerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    if (DateTime.Compare(requestTask.StartDate, requestTask.Deadline) >= 0)
                    {
                        TempData["messageerr"] = "Data de inceput trebuie sa fie inainte de cea de final";
                        return Redirect("/Tasks/Edit/" + id);
                    }

                    if (ModelState.IsValid)
                    {
                        task.Title = requestTask.Title;
                        task.Description = requestTask.Description;
                        task.StartDate = requestTask.StartDate;
                        task.Deadline = requestTask.Deadline;

                        TempData["message"] = "Task actualizat!";
                        db.SaveChanges();
                        return Redirect("/Projects/Show/" + task.ProjectId);
                    }
                    else
                    {
                        return View(requestTask);
                    }
                }
                else
                {
                    TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                    ViewBag.Message = TempData["message"];
                    return Redirect("/Projects/Index");
                }
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var userid = _userManager.GetUserId(User);
            Task task = db.Tasks.Include("Comments").Include("Project").Where(tsk => tsk.Id == id).First();
            if (userid == task.Project.ManagerId || User.IsInRole("Admin"))
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "Taskul a fost sters";
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces sa accesezi aceste informatii!";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }


    }
}