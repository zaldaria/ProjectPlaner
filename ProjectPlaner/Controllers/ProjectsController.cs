using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;
using ProjectPlaner.Models.Entity;
using System.Threading.Tasks;


namespace ProjectPlaner.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProjectsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Project> applicationDbContext;
            applicationDbContext = _context.projects.Include(t => t.client).Include(t => t.user);

            return View(await applicationDbContext.ToListAsync());            
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.projects
                .Include(p => p.client)
                .Include(p => p.user)
                .FirstOrDefaultAsync(m => m.projectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            ViewData["clientId"] = new SelectList(_context.clients, "clientId", "name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([Bind("projectId,name,clientId,comment,deadline,description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.projectId = Guid.NewGuid();

                var currentUser = await _userManager.GetUserAsync(User);

                project.userId = currentUser.Id;
                project.user = currentUser;

                _context.Add(project);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Index", "Account");
            }
            ViewData["clientId"] = new SelectList(_context.clients, "clientId", "name", project.clientId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["clientId"] = new SelectList(_context.clients, "clientId", "name", project.clientId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id, [Bind("projectId,name,clientId,comment,deadline,description")] Project project)
        {
            if (id != project.projectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.projectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["clientId"] = new SelectList(_context.clients, "clientId", "clientId", project.clientId);
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.projects
                .Include(p => p.client)
                .FirstOrDefaultAsync(m => m.projectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _context.projects.FindAsync(id);
            if (project != null)
            {
                _context.projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(Guid id)
        {
            return _context.projects.Any(e => e.projectId == id);
        }
    }
}
