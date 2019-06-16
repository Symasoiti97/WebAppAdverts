using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BusinessLogic.DataManager;
using DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using WebAppAdverts.Models;

namespace WebAppAdverts.Controllers
{
    public class HomeController : Controller
    {
        private ConcreteOperationDb _operationDb;

        public HomeController(ConcreteOperationDb operationDb)
        {
            _operationDb = operationDb;
        }

        public IActionResult Index(string name, string content)
        {
            IQueryable<Advert> adverts = _operationDb.GetAdvertisements();

            if (!String.IsNullOrEmpty(name))
            {
                adverts = adverts.Where(adv => adv.User.Name.Contains(name));
            }

            if (!String.IsNullOrEmpty(content))
            {
                adverts = adverts.Where(adv => adv.Content.Contains(content));
            }

            IndexViewModel indexVM = new IndexViewModel
            {
                Adverts = adverts
            };

            return View(indexVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel createVM)
        {
            Advert advertisement = new Advert
            {
                Content = createVM.Content,
                DateTime = DateTime.Now,
                Rating = 0,
                Number = 0,
                UserId = new Guid("58222fde-d3f2-4eb3-997f-08d6f101052e")
            };

            if (createVM.Image != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(createVM.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)createVM.Image.Length);
                }

                advertisement.Image = imageData;
            }

            _operationDb.AddAdvert(advertisement);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid advertId)
        {
            var editAdvert = _operationDb.GetAdvertisements().Where(adv => adv.Id == advertId).FirstOrDefault();

            CreateViewModel advertVM = new CreateViewModel
            {
                AdvertId = editAdvert.Id,
                Content = editAdvert.Content
            };

            return View(advertVM);
        }

        [HttpPost]
        public IActionResult Edit(CreateViewModel advertVM)
        {
            Advert advert = _operationDb.GetAdvertisements().FirstOrDefault(adv => adv.Id == advertVM.AdvertId);
            advert.Content = advertVM.Content;

            if (advertVM.Image != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(advertVM.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)advertVM.Image.Length);
                }

                advert.Image = imageData;
            }

            _operationDb.UpdateAdbert(advert);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
