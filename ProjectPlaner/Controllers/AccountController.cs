using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;
//using ProjectPlaner.Models;
//using ProjectPlaner.Models.Entity; // Ensure Task entity is accessible
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims; // Required for User.FindFirstValue
//using System.Threading.Tasks;

namespace ProjectPlaner.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager) 
        {
            _context = context;
            _userManager = userManager; 
        }

        public async Task<IActionResult> Index()
        {
            // Get the ID of the currently logged-in user
            var userId = _userManager.GetUserId(User); // GetUserId is synchronous and efficient here

            // Get now's date for comparison
            DateTime now = DateTime.Now;

            // Get all tasks for the current user and categorize them
            // Include project to avoid N+1 queries if you display project names
            var userTasks = await _context.tasks
                                          .Where(t => t.userId == userId) // Filter by current user's ID
                                          .Include(t => t.project) // Include project data
                                          .ToListAsync();

            var projects = await _context.projects.ToListAsync(); // Projects might not need to be user-specific here, depends on your design

            // Filter and sort tasks based on the current user's tasks
            var overdueTasks = userTasks
                .Where(t => t.time_limit < now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ToList();

            var todayTasks = userTasks
                .Where(t => t.time_limit.Date == now.Date && !t.IsCompleted() && t.time_limit.TimeOfDay > now.TimeOfDay)
                .OrderBy(t => t.time_limit)
                .ToList();

            var upcomingTasks = userTasks
                .Where(t => t.time_limit > now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ToList();

            var completedTasks = userTasks
                .Where(t => t.IsCompleted())
                .OrderByDescending(t => t.time_limit)
                .ToList();

            ViewBag.cnt = userTasks.Count();

            ViewBag.OverdueTasks = overdueTasks;
            ViewBag.TodayTasks = todayTasks;
            ViewBag.UpcomingTasks = upcomingTasks;
            ViewBag.CompletedTasks = completedTasks;

            ViewBag.Projects = projects; // This will show all projects, consider filtering if projects are user-specific

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                var userId = _userManager.GetUserId(User); // Get current user's ID
                if (userId == null)
                {
                    // This should ideally not happen due to [Authorize] but good for robustness
                    return Unauthorized();
                }

                var newTask = new Models.Entity.Task
                {
                    taskId = Guid.NewGuid(), // Generate new GUID for TaskId
                    name = title,
                    time_limit = DateTime.Now,
                    userId = userId, // Assign the current user's ID to the new task
                    // You might want to set default status and marker here if not done in Task constructor
                    status = Models.Entity.Task.TaskStatus.NotSet, // Assuming default status is NotSet
                    marker = Models.Entity.Task.TaskMarker.NotSet // Assuming default marker is NotSet
                };
                _context.tasks.Add(newTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsComplete(Guid id) // Changed int id to Guid id to match Task.taskId
        {
            var userId = _userManager.GetUserId(User); // Get current user's ID
            var task = await _context.tasks.Where(t => t.taskId == id && t.userId == userId).FirstOrDefaultAsync(); // Filter by TaskId AND UserId
            if (task != null)
            {
                task.SetCompleted();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ReassignTask(Guid id, DateTime newDate) // Changed int id to Guid id to match Task.taskId
        {
            var userId = _userManager.GetUserId(User); // Get current user's ID
            var task = await _context.tasks.Where(t => t.taskId == id && t.userId == userId).FirstOrDefaultAsync(); // Filter by TaskId AND UserId
            if (task != null)
            {
                task.time_limit = newDate;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}