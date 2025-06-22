using BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class ForumContext:IDb<Forum,int>
{
    private readonly CodeHubDbContext _dbContext;

    public ForumContext(CodeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(Forum item)
    {
        await _dbContext.Forums.AddAsync(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Forum> Read(int key,bool useNavigationalProperties = false, bool isReadOnly = false)
    {
        IQueryable<Forum> query = _dbContext.Forums;
        if (useNavigationalProperties) query = query.Include(f => f.Author).Include(f=>f.Comments).ThenInclude(c=>c.Author);
        if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

        Forum exercise = await query.FirstOrDefaultAsync(r => r.Id == key);

        if (exercise == null) throw new ArgumentException($"Exercise with id = {key} does not exist!");

        return exercise;
    }

    public async Task<List<Forum>> ReadAll(bool useNavigationalProperties = false,bool isReadOnly = false)
    {
        IQueryable<Forum> query = _dbContext.Forums;
        if (useNavigationalProperties) query = query.Include(f => f.Author).Include(f=>f.Comments).ThenInclude(c=>c.Author);

        if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

        return await query.ToListAsync();
    }

    public async Task Update(Forum item,bool useNavigationalProperties = false)
    {
        Forum forumFromDb = await Read(item.Id);

        _dbContext.Entry<Forum>(forumFromDb).CurrentValues.SetValues(item);
        if (useNavigationalProperties)
        {
            List<Comment> comments = new List<Comment>(item.Comments.Count);
            foreach (var comment in item.Comments)
            {
                var commentFromDb = await _dbContext.Comments.FirstOrDefaultAsync(c=>c.Id==comment.Id);
                if (commentFromDb != null) comments.Add(commentFromDb);
                else comments.Add(comment);
            }
            item.Comments = comments;
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(int key)
    {
        Forum forumFromDb = await Read(key);
        _dbContext.Forums.Remove(forumFromDb);
        await _dbContext.SaveChangesAsync();
    }
}
