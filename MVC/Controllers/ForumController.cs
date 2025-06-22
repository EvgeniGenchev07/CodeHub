using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace CodeHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly IDb<Forum, int> _forumContext;
        private readonly IdentityContext _identityContext;
        private const int forumPageSize = 10;
        public ForumController(ForumContext forumContext,IdentityContext context)
        {
            _forumContext = forumContext;
            _identityContext = context;
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
            ViewBag.TotalPages = (int)Math.Ceiling(forums.Count / (double)forumPageSize);
            var pagedForums = forums.Skip((page - 1) * forumPageSize).Take(forumPageSize).ToList();
            return View(pagedForums);
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
        [HttpPost]
        public async Task<IActionResult> Details(int? id,IFormCollection data)
        {
            if (id != data["post-id"]) NotFound();
            string content = data["comment"];
            User user = await _identityContext.ReadUserAsync(User.Identity.GetUserId<string>());
            var forum = await _forumContext.Read(id.Value, true);
            if (forum == null)
            {
                return NotFound();
            }

            Comment comment = new Comment(user,content, forum);
            forum.Comments.Add(comment);
            await _forumContext.Update(forum,true);
            return RedirectToAction(nameof(Details), new { id = id });
        }
        [Authorize(Roles = "Administrator,  User")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [Authorize(Roles = "Administrator,  User")]
        [HttpPost]
        public async Task<IActionResult> Create(Forum forum)
        {
            if (ModelState.IsValid)
            {
                User user = await _identityContext.ReadUserAsync(User.Identity.GetUserId());
                if (user == null) return Redirect("/Identity/Account/Login?ReturnUrl=/Forum/Create");
                forum.Author = user;
                forum.Date = DateTime.Now;
                await _forumContext.Create(forum);
                return RedirectToAction(nameof(Index));
            }
            return View(forum);
        }
    }
}
