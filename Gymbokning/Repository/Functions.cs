using Gymbokning.Controllers;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gymbokning.Repository
{
    public class MyFunctions : GymClassesController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyFunctions(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsBooked(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.ApplicationUserGymClasses.FindAsync(id, userId);

            return (booking != null);
        }
    }
}
