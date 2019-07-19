using DataBase.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogic.DataManager
{
    public interface IOperationDb
    {
        void CreateModel<T>(T model) where T : class, IEntity;
        IQueryable<T> GetModels<T>() where T : class, IEntity;
        IQueryable<T> GetModels<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;
    }
}
