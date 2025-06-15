using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly IDb<Forum, int> _forumContext;
        public ForumController(ForumContext forumContext)
        {
            _forumContext = forumContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _forumContext.ReadAll());
        }
    }
}
