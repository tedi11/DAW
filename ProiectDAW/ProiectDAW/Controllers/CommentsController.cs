using ProiectDAW.Data;
using Microsoft.AspNetCore.Mvc;
using ProiectDAW.Models;
using Comment = ProiectDAW.Models.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ProiectDAW.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Edit(int id)
        {

            Comment comment = db.Comments.Include("Task.Project").Where(comm => comm.Id == id).First();
            var userid = _userManager.GetUserId(User);
            if (comment.UserId == userid || User.IsInRole("Admin") || comment.Task.Project.ManagerId == userid)
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");

            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Include("Task.Project").Where(comm => comm.Id == id).First();
            var userid = _userManager.GetUserId(User);
            if (comment.UserId == userid || User.IsInRole("Admin") || comment.Task.Project.ManagerId == userid)
            {

                if (ModelState.IsValid)
                {
                    Comment comm = db.Comments.Find(id);
                    comm.Message = requestComment.Message;

                    TempData["message"] = "Comentariul a fost modificat";
                    db.SaveChanges();
                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }

        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Include("Task.Project").Where(comm => comm.Id == id).First();
            var userid = _userManager.GetUserId(User);
            if (comment.UserId == userid || User.IsInRole("Admin") || comment.Task.Project.ManagerId == userid)
            {
                Comment comm = db.Comments.Find(id);
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters";

                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }

        }

    }
}