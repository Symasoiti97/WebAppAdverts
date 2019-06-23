using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BusinessLogic.DataManager
{
    public class ConcreteOperationDb : IConcreteOperationDb
    {
        private readonly IOperationDb _db;
        private readonly ILogger _logger;

        public ConcreteOperationDb(IOperationDb db, ILogger<ConcreteOperationDb> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void AddUser(User user)
        {
            _db.CreateModel<User>(user);
            _logger.LogInformation($"DataBase | Add User | {DateTime.Now.ToShortTimeString()}");
        }

        public void DeleteUser(User user)
        {
            _db.RemoveModel<User>(user);
            _logger.LogInformation($"DataBase | Delete User | {DateTime.Now.ToShortTimeString()}");
        }

        public IQueryable<User> GetUsers()
        {
            _logger.LogInformation($"DataBase | Get Users | {DateTime.Now.ToShortTimeString()}");
            return _db.GetModels<User>();
        }

        public void AddAdvert(Advert advert)
        {
            _db.CreateModel<Advert>(advert);
            _logger.LogInformation($"DataBase | Add Advert | {DateTime.Now.ToShortTimeString()}");
        }

        public void DeleteAdvert(Advert advert)
        {
            _db.RemoveModel<Advert>(advert);
            _logger.LogInformation($"DataBase | Delete Advert | {DateTime.Now.ToShortTimeString()}");
        }

        public IQueryable<Advert> GetAdverts()
        {
            _logger.LogInformation($"DataBase | Get Adverts | {DateTime.Now.ToShortTimeString()}");
            return _db.GetModels<Advert>().Include(x => x.User);
        }

        public void UpdateAdvert(Advert advert)
        {
            _db.UpdateModel<Advert>(advert);
            _logger.LogInformation($"DataBase | Update Advert | {DateTime.Now.ToShortTimeString()}");
        }
    }
}
