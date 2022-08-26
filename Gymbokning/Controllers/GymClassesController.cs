using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Gymbokning.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            var gymClasses = await _context.GymClasses.ToListAsync();
            var gymClassesList = new List<GymClassIndexViewModel>();

            foreach (var gymClass in gymClasses)
            {
                if (gymClass.StartTime < DateTime.Now) continue;

                var gymPassesViewModel = new GymClassIndexViewModel
                {
                    Id = gymClass.Id,                       // testa automapper
                    Name = gymClass.Name,
                    StartTime = gymClass.StartTime,
                    Duration = gymClass.Duration,
                    Description = gymClass.Description,
                    IsBooked = await IsBooked(gymClass.Id) 
                };
                gymClassesList.Add(gymPassesViewModel);
            }

            return View(gymClassesList);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexBooked()
        {
            var gymClasses = await _context.GymClasses.ToListAsync();
            var gymClassesList = new List<GymClassIndexViewModel>();

            foreach (var gymClass in gymClasses)
            {
                if (gymClass.StartTime < DateTime.Now && await IsBooked(gymClass.Id)) continue;

                var gymPassesViewModel = new GymClassIndexViewModel
                {
                    Id = gymClass.Id,                       // testa automapper
                    Name = gymClass.Name,
                    StartTime = gymClass.StartTime,
                    Duration = gymClass.Duration,
                    Description = gymClass.Description,
                    IsBooked = await IsBooked(gymClass.Id)
                };
                gymClassesList.Add(gymPassesViewModel);
            }

            return View(gymClassesList);
        }

        // GET: GymClasses/Details/5
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClasses == null) return NotFound();

            //var gymClass = await _context.GymClasses.FirstOrDefaultAsync(m => m.Id == id);
            //if (gymClass == null) return NotFound();

            //var classMembersList = new List<ApplicationUser>();
            //var classMembers = _context.ApplicationUserGymClasses.Where(c => c.GymClassId == id).ToList();
            //foreach (var classMember in classMembers)
            //{
            //    var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == classMember.ApplicationUserId);
            //    classMembersList.Add(user);
            //}

            //var gymPassDetailViewModel = new GymClassDetailViewModel
            //{
            //    Id = gymClass.Id,
            //    Name = gymClass.Name,
            //    StartTime = gymClass.StartTime,
            //    Duration = gymClass.Duration,
            //    Description = gymClass.Description,
            //    GymClassMembers = classMembersList
            //};

            var gymPassDetailViewModel = await _context.GymClasses
                .Include(g => g.GymClassMembers)
                .ThenInclude(g => g.ApplicationUser)
                .Select(d => new GymClassDetailViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    StartTime = d.StartTime,
                    Duration = d.Duration,
                    Description = d.Description,
                    GymClassMembers = d.GymClassMembers.Select(a => a.ApplicationUser).ToList()
                })
                .FirstOrDefaultAsync(g => g.Id == id);

            return View(gymPassDetailViewModel);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClasses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
            }
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
          return (_context.GymClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);              // User = inloggad user (från cookie)!
            var booking = await _context.ApplicationUserGymClasses.FindAsync(id, userId);

            if (booking == null)
            {
                booking = new ApplicationUserGymClass               // Skapa bookning om den inte finns
                {
                    GymClassId = (int)id,
                    ApplicationUserId = userId
                };
                _context.ApplicationUserGymClasses.Add(booking);    // Kom ihåg Add ...
            }
            else
            {
                _context.Remove(booking);                           // Ta bort bookningen om den finns
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> IsBooked(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.ApplicationUserGymClasses.FindAsync(id, userId);

            return (booking != null);
        }

    }
}
