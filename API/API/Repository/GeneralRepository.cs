using API.Context;
using API.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public class GeneralRepository<Context, Entity, Key> : IRepository<Entity, Key>
        where Entity : class
        where Context : MyContext
    {
        private readonly MyContext context;
        private readonly DbSet<Entity> entities;
        public GeneralRepository(MyContext context)
        {
            this.context = context;
            entities = context.Set<Entity>();
        }

        public int Delete(Key key)
        {
            var entity = context.Employees.Find(key);
            context.Remove(entity);
            var result = context.SaveChanges();
            return result;
        }

        public IEnumerable<Entity> Get()
        {
            return entities.ToList();
        }

        public Entity Get(Key key)
        {
            return entities.Find(key);
        }

        public int Insert(Entity entity, Key key)
        {
            entities.Add(entity);
            var result = context.SaveChanges();
            return result;
        }

        public int Update(Entity entity, Key key)
        {
            context.Entry(entity).State = EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }
    }
}