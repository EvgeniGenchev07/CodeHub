using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using DataLayer;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CodeHub.Controllers
{
    public class BattlesController : Controller
    {
        private readonly BattlesContext _battlesContext;
        private readonly IHubContext<BattleHub> _hubContext;

        public BattlesController(BattlesContext battlesContext, IHubContext<BattleHub> hubContext)
        {
            _battlesContext = battlesContext;
            _hubContext = hubContext;
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

        public async Task<IActionResult> Details(int id)
        {
            var battle = await _battlesContext.Read(id, useNavigationalProperties: true);
            if (battle == null) return NotFound();
            return View(battle);
        }

        [HttpGet]
        public async Task<IActionResult> Join(int id)
        {
            var battle = await _battlesContext.Read(id, useNavigationalProperties: true);
            if (battle == null) return NotFound();
            return View(battle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id, string? dummy = null)
        {
            // TODO: Add join logic (e.g., add user to battle participants)
            // Notify all clients in the battle group
            await _hubContext.Clients.Group($"battle-{id}").SendAsync("PlayerJoined", User.Identity?.Name ?? "Потребител");
            return RedirectToAction("Details", new { id });
        }
    }
}