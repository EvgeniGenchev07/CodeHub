using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace CodeHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly IDb<Forum, int> _forumContext;
        private const int ForumPageSize = 1;
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
            ViewBag.TotalPages = (int)Math.Ceiling(forums.Count / (double)ForumPageSize);
            var pagedForums = forums.Skip((page - 1) * ForumPageSize).Take(ForumPageSize).ToList();
            return View(pagedForums);
        }
        public IActionResult Create()
        {
            return View();
        }

        public async  Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var forum = await _forumContext.Read(id.Value, true, true);
                if (forum == null)
                {
                    return NotFound();
                }

                forum.Views++;
                await _forumContext.Update(forum);
                return View(forum);
            }
            return RedirectToAction();
        }
    }
}
