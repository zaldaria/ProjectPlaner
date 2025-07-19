using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Data;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // GET: Users/ChangeRole/5
    public async Task<IActionResult> ChangeRole(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (user.Id == currentUserId)
        {
            TempData["ErrorMessage"] = "You can`t change your Role!";
            return RedirectToAction(nameof(Index));
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        ViewBag.UserId = user.Id;
        ViewBag.UserName = user.UserName;
        ViewBag.UserEmail = user.Email;
        ViewBag.CurrentRoles = userRoles.ToList();
        ViewBag.AllRoles = allRoles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name,
            Selected = userRoles.Contains(r.Name)
        }).ToList();

        return View();
    }

    // POST: Users/ChangeRole/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string userId, [FromForm] string[] selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (user.Id == currentUserId)
        {
            ModelState.AddModelError(string.Empty, "You can`t change your Role!");            
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;
            ViewBag.CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();
            ViewBag.AllRoles = (await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
                Selected = selectedRoles.Contains(r.Name)
            }).ToListAsync());
            return View();
        }

        if (selectedRoles == null || selectedRoles.Length == 0)
        {
            ModelState.AddModelError("selectedRoles", "Choose at least one role.");           
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;
            ViewBag.CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();
            ViewBag.AllRoles = (await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
                Selected = false
            }).ToListAsync());
            return View();
        }
        try
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var addedRoles = selectedRoles.Except(userRoles);
            var removedRoles = userRoles.Except(selectedRoles);

            var addResult = await _userManager.AddToRolesAsync(user, addedRoles);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, removedRoles);

            if (!addResult.Succeeded || !removeResult.Succeeded)
            {
                foreach (var error in addResult.Errors.Concat(removeResult.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                ViewBag.UserId = user.Id;
                ViewBag.UserName = user.UserName;
                ViewBag.UserEmail = user.Email;
                ViewBag.CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();
                ViewBag.AllRoles = (await _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = selectedRoles.Contains(r.Name)
                }).ToListAsync());
                return View();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error while saving roles: {ex.Message}");
            
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;
            ViewBag.CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();
            ViewBag.AllRoles = (await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
                Selected = selectedRoles.Contains(r.Name)
            }).ToListAsync());
            return View();
        }

        TempData["SuccessMessage"] = "Roles update success";
        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string userId)
    {
        var userToDelete = await _userManager.FindByIdAsync(userId);
        if (userToDelete == null)
        {
            return NotFound();
        }

        var allTasksToDelete = await _context.tasks.Where(t => t.userId == userId).ToListAsync();
        _context.tasks.RemoveRange(allTasksToDelete);
        await _context.SaveChangesAsync(); 
        
        var allProjectsToDelete = await _context.projects.Where(p => p.userId == userId).ToListAsync();
        _context.projects.RemoveRange(allProjectsToDelete);
        await _context.SaveChangesAsync();

        var allClientsToDelete = await _context.clients.Where(c => c.userId == userId).ToListAsync();
        _context.clients.RemoveRange(allClientsToDelete);
        await _context.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(userToDelete);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Users");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(userToDelete);
        }
    }
}