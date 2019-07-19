using DataBase;
using DataBase.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogic.DataManager
{
    public class OperationDb : IOperationDb
    {
        private readonly ApplicationContext _db;

        public OperationDb(ApplicationContext context)
        {
            _db = context;
        }

        public void CreateModel<T>(T model) where T : class, IEntity
        {
            _db.Set<T>().Add(model);
            _db.SaveChanges();
        }

        public IQueryable<T> GetModels<T>() where T : class, IEntity
        {
            return _db.Set<T>();
        }

        public IQueryable<T> GetModels<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return _db.Set<T>().Where(predicate);
        }
    }
}
