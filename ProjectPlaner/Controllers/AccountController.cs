using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;

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
            var userId = _userManager.GetUserId(User); 

            DateTime now = DateTime.Now;

            var userTasks = await _context.tasks
                                          .Where(t => t.userId == userId) 
                                          .Include(t => t.project) 
                                          .ToListAsync();
            
            var projects = await _context.projects
                                         .Where(p => p.userId == userId) 
                                         .ToListAsync();
            var clients = await _context.clients
                                         .Where(c => c.userId == userId) 
                                         .ToListAsync();

            var overdueTasks = userTasks
                .Where(t => t.time_limit < now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            var todayTasks = userTasks
                .Where(t => t.time_limit.Date == now.Date && !t.IsCompleted() && t.time_limit.TimeOfDay > now.TimeOfDay)
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            var upcomingTasks = userTasks
                .Where(t => t.time_limit.Date > now.Date && t.time_limit > now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            var completedTasks = userTasks
                .Where(t => t.IsCompleted())
                .OrderByDescending(t => t.time_limit)
                .ToList();

            ViewBag.cnt = userTasks.Count();
            ViewBag.UserId = userId;

            ViewBag.UserTasks = userTasks;
            ViewBag.OverdueTasks = overdueTasks;
            ViewBag.TodayTasks = todayTasks;
            ViewBag.UpcomingTasks = upcomingTasks;
            ViewBag.CompletedTasks = completedTasks;

            ViewBag.Projects = projects;
            ViewBag.Clients = clients;

            return View();
        }

        public async Task<IActionResult> ProjectTasks(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            DateTime now = DateTime.Now;
            
            var projectTasks = await _context.tasks
                                            .Where(t => t.projectId == id && t.userId == userId)
                                            .Include(t => t.project)
                                            .ToListAsync();
 
            ViewBag.OverdueTasks = projectTasks
                .Where(t => t.time_limit < now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            ViewBag.TodayTasks = projectTasks
                .Where(t => t.time_limit.Date == now.Date && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            ViewBag.UpcomingTasks = projectTasks
                .Where(t => t.time_limit.Date > now.Date && t.time_limit > now && !t.IsCompleted())
                .OrderBy(t => t.time_limit)
                .ThenBy(t => t.marker)
                .ToList();

            ViewBag.CompletedTasks = projectTasks
                .Where(t => t.IsCompleted())
                .OrderByDescending(t => t.time_limit)
                .ToList();

            var projects = await _context.projects
                                         .Where(p => p.userId == userId)
                                         .ToListAsync();
            var clients = await _context.clients
                                         .Where(c => c.userId == userId)
                                         .ToListAsync();

            ViewBag.CurrentProject = projects.Where(p => p.projectId == id).FirstOrDefault();
            ViewBag.CurrentProjectId = id;
            ViewBag.Projects = projects;
            ViewBag.Clients = clients;

            ViewBag.IsProjectView = true;
            ViewBag.IsClientView = false;
            return View("Index");
        }

        public async Task<IActionResult> ClientInfo(Guid id)
        {
            ViewBag.IsClientView = true;

            var client = await _context.clients
                                         .Where(c => c.clientId == id)
                                         .FirstOrDefaultAsync();
            ViewBag.CurrentClient = client;
            ViewBag.CurrentClientId = client?.clientId;

            var userId = _userManager.GetUserId(User);

            var projects = await _context.projects
                                         .Where(p => p.userId == userId)
                                         .ToListAsync();
            var clients = await _context.clients
                                         .Where(c => c.userId == userId)
                                         .ToListAsync();
            ViewBag.Projects = projects;
            ViewBag.Clients = clients;
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsComplete(Guid id) 
        {
            var userId = _userManager.GetUserId(User); 
            var task = await _context.tasks.Where(t => t.taskId == id && t.userId == userId).FirstOrDefaultAsync(); // Filter by TaskId AND UserId
            if (task != null)
            {
                task.SetCompleted();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

       
    }
}