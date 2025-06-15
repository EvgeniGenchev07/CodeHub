using System;
using System.Collections.Generic;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer
{
    public class BattlesContext : IDb<Battle, int>
    {
        private readonly CodeHubDbContext dbContext;

        public BattlesContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(Battle item)
        {
            dbContext.Battles.Add(item);
            dbContext.SaveChanges();
        }

        public Battle Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Battle> query = dbContext.Battles;

            if (useNavigationalProperties)
                query = query
                    .Include(b => b.FirstPlayer)
                    .Include(b => b.SecondPlayer);

            if (isReadOnly)
                query = query.AsNoTrackingWithIdentityResolution();

            Battle battle = query.FirstOrDefault(b => b.Id == key);

            if (battle == null)
                throw new ArgumentException($"Battle with ID = {key} does not exist!");

            return battle;
        }

        public List<Battle> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Battle> query = dbContext.Battles;

            if (useNavigationalProperties)
                query = query
                    .Include(b => b.FirstPlayer)
                    .Include(b => b.SecondPlayer);

            if (isReadOnly)
                query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public void Update(Battle item, bool useNavigationalProperties = false)
        {
            dbContext.SaveChanges();
        }

        public void Delete(int key)
        {
            Battle battle = Read(key);
            dbContext.Battles.Remove(battle);
            dbContext.SaveChanges();
        }
    }
}
