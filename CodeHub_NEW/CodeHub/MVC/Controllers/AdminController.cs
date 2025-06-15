using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return PartialView();
        }
    }
}
