using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using DataLayer;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHub.Controllers
{
    public class BattlesController : Controller
    {
        private readonly BattlesContext _battlesContext;

        public BattlesController(BattlesContext battlesContext)
        {
            _battlesContext = battlesContext;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var battles = await _battlesContext.ReadAll();

            var model = new
            {
                ActiveBattles = battles.Where(b => b.StartDate <= now && b.EndDate >= now).ToList(),
                UpcomingBattles = battles.Where(b => b.StartDate > now).ToList(),
                PastBattles = battles.Where(b => b.EndDate < now).ToList()
            };

            return View(model);
        }
    }
}