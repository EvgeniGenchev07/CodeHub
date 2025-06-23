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

        public async Task Create(Battle item)
        {
            try
            {
                await dbContext.Battles.AddAsync(item);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to create battle. Possible invalid data or missing players.", ex);
            }
        }

        public async Task<Battle> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Battle> query = dbContext.Battles;

            if (useNavigationalProperties)
            {
                query = query
                    .Include(b => b.FirstPlayer)
                    .Include(b => b.SecondPlayer);
            }

            if (isReadOnly)
            {
                query = query.AsNoTracking();
            }

            var battle = await query.FirstOrDefaultAsync(b => b.Id == key);

            return battle ?? throw new KeyNotFoundException($"Battle with ID {key} not found");
        }

        public async Task<List<Battle>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Battle> query = dbContext.Battles;

            if (useNavigationalProperties)
            {
                query = query
                    .Include(b => b.FirstPlayer)
                    .Include(b => b.SecondPlayer);
            }

            if (isReadOnly)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task Update(Battle item, bool useNavigationalProperties = false)
        {
            try
            {
                var existingBattle = await Read(item.Id, useNavigationalProperties);

                dbContext.Entry(existingBattle).CurrentValues.SetValues(item);

                if (useNavigationalProperties)
                {
                    existingBattle.FirstPlayer = item.FirstPlayer;
                    existingBattle.SecondPlayer = item.SecondPlayer;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Battle was modified or deleted by another user.", ex);
            }
        }

        public async Task Delete(int key)
        {
            var battle = await Read(key);
            dbContext.Battles.Remove(battle);
            await dbContext.SaveChangesAsync();
        }

    }
}
