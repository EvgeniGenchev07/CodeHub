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
        public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] Filters filter = Filters.All,string search = null)
        {
            var forums = await _forumContext.ReadAll(true,true);
            ViewBag.Filter = filter;
            ViewBag.Page = page;
            ViewBag.Search = search;
            if (filter != Filters.All)
            {
                forums = forums.Where(f => f.Filters.Any(ff => ff == filter)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                forums = forums.Where(f => f.Title.ToLower().Contains(search.ToLower())).ToList();
            }
            var pagedForums = forums.Skip((page - 1) * ForumPageSize).Take(ForumPageSize).ToList();
            return View(pagedForums);
        }
    }
}
