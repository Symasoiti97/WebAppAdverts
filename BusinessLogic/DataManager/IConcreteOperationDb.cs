using DataBase.Models;
using System.Linq;

namespace BusinessLogic.DataManager
{
    public interface IConcreteOperationDb
    {
        void AddUser(User user);
        void DeleteUser(User user);
        IQueryable<User> GetUsers();
        void AddAdvert(Advert advert);
        void DeleteAdvert(Advert advert);
        IQueryable<Advert> GetAdverts();
        void UpdateAdvert(Advert advert);
    }
}
