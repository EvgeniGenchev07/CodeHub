using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly IDb<Forum, int> _forumContext;
        private const int ForumPageSize = 10;
        public ForumController(ForumContext forumContext)
        {
            _forumContext = forumContext;
        }
        public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] ForumFilters filter = ForumFilters.All)
        {
            var forums = await _forumContext.ReadAll();
            forums = forums.Where(f=>f.Filters.Any(ff=>ff==filter)).ToList();
            var pagedForums = forums.Skip((page - 1) * ForumPageSize).Take(ForumPageSize).ToList();
            return View(pagedForums);
        }
    }
}
