using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BusinessLogic.DataManager
{
    public class ConcreteOperationDb
    {
        private OperationDb _db;

        public ConcreteOperationDb(OperationDb db)
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

        public void AddAdvert(Advert advertisement)
        {
            _db.CreateModel<Advert>(advertisement);
        }

        public void DeleteAdvert(Advert advertDelete)
        {
            _db.RemoveModel<Advert>(advertDelete);
        }

        public IQueryable<Advert> GetAdvertisements()
        {
            return _db.GetModels<Advert>().Include(x => x.User);
        }

        public void UpdateAdbert(Advert advertisement)
        {
            _db.UpdateModel<Advert>(advertisement);
        }
    }
}
