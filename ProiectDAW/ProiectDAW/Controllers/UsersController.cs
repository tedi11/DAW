﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.Data;
using ProiectDAW.Models;

namespace ProiectDAW.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Show(string id)
        {
            AppUser user = db.Users.Find(id);
            if (user == null)
            {
                TempData["message"] = "Eroare BD!";
                return RedirectToAction("Index");
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);

                ViewBag.Roles = roles;
                ViewBag.UserCurent = _userManager.GetUserId(User);

                return View(user);
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = db.Users.Find(id);
            if (user == null)
            {
                TempData["message"] = "Eroare BD!";
                return RedirectToAction("Index");
            }
            else
            {

                user.AllRoles = GetAllRoles();

                var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

                // Cautam ID-ul rolului in baza de date
                var currentUserRole = _roleManager.Roles
                                                  .Where(r => roleNames.Contains(r.Name))
                                                  .Select(r => r.Id)
                                                  .First(); // Selectam 1 singur rol
                ViewBag.UserRole = currentUserRole;

                return View(user);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, AppUser newData, [FromForm] string newRole)
        {
            AppUser user = db.Users.Find(id);
            if (user == null)
            {
                TempData["message"] = "Eroare BD!";
                return RedirectToAction("Index");
            }
            else
            {

                user.AllRoles = GetAllRoles();


                if (ModelState.IsValid)
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    user.PhoneNumber = newData.PhoneNumber;

                    var roles = db.Roles.ToList();
                    foreach (var role in roles)
                    {
                        // Scoatem userul din rolurile anterioare
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    // Adaugam noul rol selectat
                    var roleName = await _roleManager.FindByIdAsync(newRole);
                    await _userManager.AddToRoleAsync(user, roleName.ToString());

                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var isAdmin = User.IsInRole("Admin");
            var isUser = id == _userManager.GetUserId(User);
            if (isAdmin || isUser)
            {
                var user = db.Users
                             .Include("Projects.Tasks.Comments")
                             .Include("Members")
                             .Include("Tasks.Comments")
                             .Include("Comments")
                             .Where(u => u.Id == id)
                             .First();


                // Delete user comments
                if (user.Comments.Count > 0)
                {
                    foreach (var comment in user.Comments)
                    {
                        db.Comments.Remove(comment);
                    }
                }
                // Delete user articles
                if (user.Tasks.Count > 0)
                {
                    foreach (var task in user.Tasks)
                    {
                        db.Tasks.Remove(task);
                    }
                }
                
                if (user.Members.Count > 0)
                {
                    foreach (var member in user.Members)
                    {
                        db.Members.Remove(member);
                    }
                }

                if (user.Projects.Count > 0)
                {
                    foreach (var project in user.Projects)
                    {
                        db.Projects.Remove(project);
                    }
                }

                db.Users.Remove(user);
                db.SaveChanges();

                if (isUser)
                    return RedirectToAction("Identity/Account/Logout");
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("/Home/Index");
            }
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                if (role.Name.ToString().ToLower() != "manager")
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = role.Id.ToString(),
                        Text = role.Name.ToString()
                    });
                }
            }
            return selectList;
        }
    }
}
