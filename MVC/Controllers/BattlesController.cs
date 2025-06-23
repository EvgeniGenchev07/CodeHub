using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using DataLayer;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CodeHub.Controllers
{
    public class BattlesController : Controller
    {
        private readonly BattlesContext _battlesContext;
        private readonly IHubContext<BattleHub> _hubContext;
        private readonly IdentityContext  _identityContext;

        public BattlesController(BattlesContext battlesContext, IHubContext<BattleHub> hubContext,IdentityContext identityContext)
        {
            _battlesContext = battlesContext;
            _hubContext = hubContext;
            _identityContext = identityContext;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _identityContext.ReadUserAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    Battle battle = await _battlesContext.Read(id.Value,true,true);
                    if (battle.FirstPlayer == null)
                    {
                        battle.FirstPlayer = user;
                    }
                    else
                    {
                        battle.SecondPlayer = user;
                    }

                    _battlesContext.Update(battle,true);
                    return View(battle);
                }
            }
            return View(await _battlesContext.Read(id.Value));
        }
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int? id)
        {
            return View();
        }
    }
}