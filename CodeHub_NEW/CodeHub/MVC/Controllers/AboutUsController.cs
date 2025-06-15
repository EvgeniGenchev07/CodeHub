using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Conditions()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
