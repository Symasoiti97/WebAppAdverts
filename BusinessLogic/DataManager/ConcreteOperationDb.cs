using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BusinessLogic.DataManager
{
    public class ConcreteOperationDb : IConcreteOperationDb
    {
        private readonly IOperationDb _db;

        public ConcreteOperationDb(IOperationDb db)
        {
            _db = db;
        }

        public void AddUser(User user)
        {
            _db.CreateModel<User>(user);
        }

        public void DeleteUser(User user)
        {
            _db.RemoveModel<User>(user);
        }

        public IQueryable<User> GetUsers()
        {
            return _db.GetModels<User>();
        }

        public void AddAdvert(Advert advert)
        {
            _db.CreateModel<Advert>(advert);
        }

        public void DeleteAdvert(Advert advert)
        {
            _db.RemoveModel<Advert>(advert);
        }

        public IQueryable<Advert> GetAdverts()
        {
            return _db.GetModels<Advert>().Include(x => x.User);
        }

        public void UpdateAdvert(Advert advert)
        {
            _db.UpdateModel<Advert>(advert);
        }
    }
}
