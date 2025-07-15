using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;
using ProjectPlaner.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectPlaner.Controllers
{
    [Authorize(Roles = "Admin,User")] // Allows both Admin and User roles to access the controller
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User); // Get current user
            // Filter tasks to show only those belonging to the current user, or all if Admin
            IQueryable<ProjectPlaner.Models.Entity.Task> applicationDbContext;
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                applicationDbContext = _context.tasks.Include(t => t.project).Include(t => t.user); // Include user for Index if needed
            }
            else
            {
                applicationDbContext = _context.tasks.Include(t => t.project)
                                                     .Where(t => t.userId == currentUser.Id); // Filter by userId
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.tasks
                .Include(t => t.project)
                .FirstOrDefaultAsync(m => m.taskId == id);
            if (task == null)
            {
                return NotFound();
            }

            // Ensure the user can only view their own task details unless they are Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
            {
                return Forbid();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("userId,user,taskId,name,projectId,status,marker,time_limit,description")] Models.Entity.Task task)
        {
            if (ModelState.IsValid)
            {
                task.taskId = Guid.NewGuid();

                var currentUser = await _userManager.GetUserAsync(User);

                task.userId = currentUser.Id;
                task.user = currentUser; // Ensure the navigation property is also set

                _context.Add(task);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Index", "Account");
            }
            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name", task.projectId);
            return View(task);
        }


        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Ensure the user can only edit their own task unless they are Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
            {
                return Forbid();
            }

            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name", task.projectId);
            return View(task);
        }


        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("userId,user,taskId,name,projectId,status,marker,time_limit,description")] Models.Entity.Task task)
        {
            if (id != task.taskId)
            {
                return NotFound();
            }

            // Retrieve the existing task from the database to get the userId and ensure it's not overwritten
            // Use AsNoTracking() because we will attach the 'task' object from the model binder later
            var existingTask = await _context.tasks.AsNoTracking().FirstOrDefaultAsync(t => t.taskId == id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // Ensure the user can only edit their own task unless they are Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && existingTask.userId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manually set userId and user from the existing task to the bound task object
                    // This prevents the userId from being nullified if it's not in the Bind attribute
                    task.userId = existingTask.userId;
                    // You generally don't need to set the navigation property 'user' on the 'task' object being updated
                    // unless you are explicitly loading it and want to re-attach it to the context,
                    // which is usually handled by EF Core when the foreign key is set.
                    // If you face issues, consider loading it: task.user = existingTask.user;

                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.taskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Index", "Account");
            }
            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name", task.projectId);
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.tasks
                .Include(t => t.project)
                .FirstOrDefaultAsync(m => m.taskId == id);
            if (task == null)
            {
                return NotFound();
            }

            // Ensure the user can only delete their own task unless they are Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
            {
                return Forbid();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var task = await _context.tasks.FindAsync(id);
            if (task != null)
            {
                // Ensure the user can only delete their own task unless they are Admin
                var currentUser = await _userManager.GetUserAsync(User);
                if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
                {
                    return Forbid();
                }

                _context.tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction("Index", "Account");
        }

        private bool TaskExists(Guid id)
        {
            return _context.tasks.Any(e => e.taskId == id);
        }
    }
}