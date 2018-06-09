using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BeeperCalculator.Models
{
    public class DbRepository<T> where T : class
    {
        private DatabaseContext context;

        public DbRepository (DatabaseContext context) {
            this.context = context;
            DbSet = context.Set<T>();
        }

        protected DbSet<T> DbSet { get; set; }

        public List<T> GetAll()
        {
            return DbSet.ToList();
        }

        public T Get(int id)
        {
            return DbSet.Find(id);
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}