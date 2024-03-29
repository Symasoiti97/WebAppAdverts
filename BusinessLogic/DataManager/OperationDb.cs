﻿using DataBase;
using DataBase.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogic.DataManager
{
    public class OperationDb
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

        public void RemoveModel<T>(T model) where T : class, IEntity
        {
            _db.Set<T>().Remove(model);
            _db.SaveChanges();
        }

        public void UpdateModel<T>(T model) where T : class, IEntity
        {
            var m = _db.Set<T>().Find(model.Id);
            m = model;
            _db.SaveChanges();
        }

        public T GetModelFirstOfDefault<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return _db.Set<T>().Where(predicate).FirstOrDefault();
        }

        public IQueryable<T> GetModels<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return _db.Set<T>().Where(predicate);
        }
    }
}
