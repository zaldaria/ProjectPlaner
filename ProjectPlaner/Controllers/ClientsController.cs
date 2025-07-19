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
using System.Threading.Tasks;

namespace ProjectPlaner.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ClientsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clients
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Client> applicationDbContext;
            applicationDbContext = _context.clients.Include(t => t.user);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.clients
                .Include(c => c.user)
                .FirstOrDefaultAsync(c => c.clientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([Bind("clientId,name,phone,email")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.clientId = Guid.NewGuid();

                var currentUser = await _userManager.GetUserAsync(User);

                client.userId = currentUser.Id;
                client.user = currentUser;

                _context.Add(client);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Index", "Account");
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id, [Bind("clientId,name,phone,email")] Client client)
        {
            if (id != client.clientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var clientToUpdate = await _context.clients.FindAsync(id);
                    if (clientToUpdate == null)
                    {
                        return NotFound();
                    }

                    clientToUpdate.name = client.name;
                    clientToUpdate.phone = client.phone;
                    clientToUpdate.email = client.email;

                    _context.Update(clientToUpdate); 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.clientId))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.clients
                .FirstOrDefaultAsync(m => m.clientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = await _context.clients.FindAsync(id);
            if (client != null)
            {
                _context.clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(Guid id)
        {
            return _context.clients.Any(e => e.clientId == id);
        }
    }
}
