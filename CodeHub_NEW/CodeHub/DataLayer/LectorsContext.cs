using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LectorsContext
    {
        CodeHubDbContext dbContext;

        public LectorsContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(Lector item)
        {
            dbContext.Lectors.Add(item);
            dbContext.SaveChanges();
        }

        public Lector Read(int key, bool isReadOnly = false)
        {
            IQueryable<Lector> query = dbContext.Lectors;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Lector lector = query.FirstOrDefault(r => r.Id == key);

            if (lector == null) throw new ArgumentException($"Lector with id = {key} does not exist!");

            return lector;
        }

        public List<Lector> ReadAll(bool isReadOnly = false)
        {
            IQueryable<Lector> query = dbContext.Lectors;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public void Update(Lector item)
        {
            Lector lectorFromDb = Read(item.Id);

            dbContext.Entry<Lector>(lectorFromDb).CurrentValues.SetValues(item);

            dbContext.SaveChanges();
        }

        public void Delete(int key)
        {
            Lector lectorFromDb = Read(key);
            dbContext.Lectors.Remove(lectorFromDb);
            dbContext.SaveChanges();
        }
    }
}
