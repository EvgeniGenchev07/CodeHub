using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LectorsContext : IDb<Lector, int>
    {
        CodeHubDbContext dbContext;

        public LectorsContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(Lector item)
        {
            dbContext.Lectors.Add(item);
            dbContext.SaveChanges();
        }

        public async Task<Lector> Read(int key, bool isReadOnly = false)
        {
            IQueryable<Lector> query = dbContext.Lectors;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Lector lector = query.FirstOrDefault(r => r.Id == key);

            if (lector == null) throw new ArgumentException($"Lector with id = {key} does not exist!");

            return lector;
        }

        public async Task<List<Lector>> ReadAll(bool isReadOnly = false)
        {
            IQueryable<Lector> query = dbContext.Lectors;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public async Task Update(Lector item)
        {
            Lector lectorFromDb = Read(item.Id);

            dbContext.Entry<Lector>(lectorFromDb).CurrentValues.SetValues(item);

            dbContext.SaveChanges();
        }

        public async Task Delete(int key)
        {
            Lector lectorFromDb = Read(key);
            dbContext.Lectors.Remove(lectorFromDb);
            dbContext.SaveChanges();
        }
    }
}
