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
        private const int ForumPageSize = 10;
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
            ViewBag.TotalPages = (int)Math.Ceiling(forums.Count / (double)ForumPageSize);
            var pagedForums = forums.Skip((page - 1) * ForumPageSize).Take(ForumPageSize).ToList();
            return View(pagedForums);
        }
        public async Task<IActionResult> Create()
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Forum post)
        {
            if (post == null)
            {
                Console.WriteLine("Received null post object");
                return BadRequest("Post object is null");
            }

            // Clear ModelState errors for fields set server-side
            ModelState.Remove("Author");
            ModelState.Remove("Date");
            // Remove Comments if it's causing validation errors (optional, based on model)
            ModelState.Remove("Comments");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                Console.WriteLine("ModelState errors: " + System.Text.Json.JsonSerializer.Serialize(errors));
                return BadRequest(errors);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _identityContext.ReadUserAsync(userId);


            var newPost = new Forum
            {
                Title = post.Title,
                Content = post.Content,
                Author = user,
                Date = DateTime.Now,
                Views = 0,
                Filters = post.Filters ?? new List<Filters>(),
                Code = post.Code,
                Comments = new List<Comment>()
            };

            await _forumContext.Create(newPost);

            return Ok(new { success = true, message = "Постът е създаден успешно!", postId = newPost.Id });
        }
    }
}
