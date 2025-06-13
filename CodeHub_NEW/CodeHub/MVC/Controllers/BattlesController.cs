using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class BattlesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
