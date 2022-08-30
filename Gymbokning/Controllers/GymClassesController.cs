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
using AutoMapper;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            this._mapper = mapper;
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
                    IsBooked = IsBooked(gymClass.Id) 
                };
                gymClassesList.Add(gymPassesViewModel);
            }

            return View(gymClassesList);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexBooked()
        {

            var userId = _userManager.GetUserId(User);

            var gymClasses = await _context.GymClasses
                .Include(g => g.GymClassMembers)
                .Where(g => g.StartTime > DateTime.Now)
                .ToListAsync();
            var gymClassesList = new List<GymClassIndexViewModel>();

            foreach (var gymClass in gymClasses)
            {
                //if (!await IsBooked(gymClass.Id)) continue;

                var gymPassesViewModel = new GymClassIndexViewModel
                {
                    Id = gymClass.Id,                       // testa automapper
                    Name = gymClass.Name,
                    StartTime = gymClass.StartTime,
                    Duration = gymClass.Duration,
                    Description = gymClass.Description,
                    IsBooked = true /*await IsBooked(gymClass.Id)*/  // alltid true
                };
                gymClassesList.Add(gymPassesViewModel);
            }

            //var gymClassesList = await _mapper.ProjectTo<GymClassIndexViewModel>(                 // Hur göra med IsBooked ?
            //    _context.GymClasses.Where(g => g.StartTime > DateTime.Now)).ToListAsync();

            return View(gymClassesList);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexPassed()
        {
            var gymClasses = await _context.GymClasses.Where(g => g.StartTime < DateTime.Now).ToListAsync();
            var gymClassesList = new List<GymClassIndexViewModel>();

            foreach (var gymClass in gymClasses)
            {
                if (!IsBooked(gymClass.Id)) continue;

                var gymPassesViewModel = new GymClassIndexViewModel
                {
                    Id = gymClass.Id,                       // testa automapper
                    Name = gymClass.Name,
                    StartTime = gymClass.StartTime,
                    Duration = gymClass.Duration,
                    Description = gymClass.Description,
                    IsBooked = true /*await IsBooked(gymClass.Id)*/  // alltid true
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

            //var gymPassDetailViewModel = await _context.GymClasses
            //    .Include(g => g.GymClassMembers)
            //    .ThenInclude(g => g.ApplicationUser)
            //    .Select(d => new GymClassDetailViewModel
            //    {
            //        Id = d.Id,
            //        Name = d.Name,
            //        StartTime = d.StartTime,
            //        Duration = d.Duration,
            //        Description = d.Description,
            //        GymClassMembers = d.GymClassMembers.Select(a => a.ApplicationUser).ToList()
            //    })
            //    .FirstOrDefaultAsync(g => g.Id == id);

            var gymPassDetailViewModel = await _mapper.ProjectTo<GymClassDetailViewModel>(_context.GymClasses)
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
            if (id == null || _context.GymClasses == null) return NotFound();

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null) return NotFound();

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
            if (id != gymClass.Id) return NotFound();

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

        public bool IsBooked(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = _context.ApplicationUserGymClasses.Find(id, userId);

            return (booking != null);
        }

        //public async Task<bool> IsBooked(int id)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var booking = await _context.ApplicationUserGymClasses.FindAsync(id, userId);

        //    return (booking != null);
        //}

        public async Task<JsonResult> ValidateStartTime(DateTime startTime)
        {
            if(startTime < DateTime.Now)
            {
                return Json("This time has allready passed");
            }
            return Json(true);
        }

        //public bool IsPassed(DateTime startTime)
        //{
        //    return (startTime < DateTime.Now);
        //}

    }
}
