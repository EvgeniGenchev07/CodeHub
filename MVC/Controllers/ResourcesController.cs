using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class ResourcesController : Controller
    {
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Partners()
        {
            return View();
        }
        public IActionResult Documentation()
        {
            return View();
        }
    }
    
}
