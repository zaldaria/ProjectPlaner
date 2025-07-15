using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;

namespace ProjectPlaner.Controllers
{
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<ProjectPlaner.Models.Entity.Task> applicationDbContext;
            applicationDbContext = _context.tasks.Include(t => t.project).Include(t => t.user);
         
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        [Authorize(Roles = "Admin,User")] 
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.tasks
                .Include(t => t.project)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.taskId == id);
            if (task == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
            {
                return Forbid();
            }

            return View(task);
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Admin,User")] 
        public IActionResult Create()
        {
            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name");
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")] 
        public async Task<IActionResult> Create([Bind("taskId,name,projectId,status,marker,time_limit,description")] Models.Entity.Task task)
        {
            if (ModelState.IsValid)
            {
                task.taskId = Guid.NewGuid();

                var currentUser = await _userManager.GetUserAsync(User);

                task.userId = currentUser.Id;
                task.user = currentUser;

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
        [Authorize(Roles = "Admin,User")]
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

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && task.userId != currentUser.Id)
            {
                return Forbid();
            }

            ViewData["projectId"] = new SelectList(_context.projects, "projectId", "name", task.projectId);
            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")] 
        public async Task<IActionResult> Edit(Guid id, [Bind("userId,user,taskId,name,projectId,status,marker,time_limit,description")] Models.Entity.Task task)
        {
            if (id != task.taskId)
            {
                return NotFound();
            }

            var existingTask = await _context.tasks.AsNoTracking().FirstOrDefaultAsync(t => t.taskId == id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && existingTask.userId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task.userId = existingTask.userId;

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
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var task = await _context.tasks.FindAsync(id);
            if (task != null)
            {
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