using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataManager;
using BusinessLogic.Options;
using BusinessLogic.Services;
using DataBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAppAdverts.Models;

namespace WebAppAdverts.Controllers
{
    public class HomeController : Controller
    {
        private ConcreteOperationDb _operationDb;
        private IReCaptchaService _reCaptcha;
        private CreateService _createAdvert;
        private readonly int _countAdvertsByAuftor;
        private readonly int _countAdvertsByPage;

        public HomeController(ConcreteOperationDb operationDb, IReCaptchaService reCaptcha, IOptions<AppOptions> options, CreateService createAdvert)
        {
            _operationDb = operationDb;
            _reCaptcha = reCaptcha;
            _countAdvertsByPage = options.Value.IndexOptions.CountAdvertsByPage;
            _countAdvertsByAuftor = options.Value.IndexOptions.CountAdvertsByAuftor;
            _createAdvert = createAdvert;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string name, string content, int page = 1, SortState sortOrder = SortState.DateDesc)
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

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    adverts = adverts.OrderByDescending(sort => sort.User.Name);
                    break;
                case SortState.NameAsc:
                    adverts = adverts.OrderBy(sort => sort.User.Name);
                    break;
                case SortState.RatingDesc:
                    adverts = adverts.OrderByDescending(sort => sort.Rating);
                    break;
                case SortState.RatingAsc:
                    adverts = adverts.OrderBy(sort => sort.Rating);
                    break;
                case SortState.DateDesc:
                    adverts = adverts.OrderByDescending(sort => sort.DateTime);
                    break;
                case SortState.DateAsc:
                    adverts = adverts.OrderBy(sort => sort.DateTime);
                    break;
            }

            var count = await adverts.CountAsync();
            var items = await adverts.Skip((page - 1) * _countAdvertsByPage).Take(_countAdvertsByPage).ToListAsync();

            IndexViewModel indexVM = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, _countAdvertsByPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(name, content),
                Adverts = items
            };

            return View(indexVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel createVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createVM);
            }
            else
            {
                var captchaResponce = await _reCaptcha.Validate(Request.Form);

                if (!captchaResponce.Success)
                {
                    ModelState.AddModelError("reCaptchaError",
                    "reCAPTCHA error occured. Please try again.");

                    return View(createVM);
                }
            }


            var advert = _createAdvert.CreateAdvert(createVM.Content, createVM.Image, new Guid("58222fde-d3f2-4eb3-997f-08d6f101052e"));

            _operationDb.AddAdvert(advert);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(Guid advertId)
        {
            var editAdvert = _operationDb.GetAdvertisements().Where(adv => adv.Id == advertId).FirstOrDefault();

            EditViewModel advertVM = new EditViewModel
            {
                AdvertId = editAdvert.Id,
                Content = editAdvert.Content,
                ImageByte = editAdvert.Image
            };

            return View(advertVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditViewModel advertVM)
        {
            if (!ModelState.IsValid)
            {
                return View(advertVM);
            }

            Advert advert = _operationDb.GetAdvertisements().FirstOrDefault(adv => adv.Id == advertVM.AdvertId);
            advert.Content = advertVM.Content;
            advert.DateTime = DateTime.Now;

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

        [HttpGet]
        public IActionResult DeleteModal(Guid advertId)
        {
            return View(advertId);
        }

        [HttpPost, ActionName("DeleteModal")]
        public IActionResult DeleteConfirmed(Guid advertId)
        {
            var advertDelete = _operationDb.GetAdvertisements().FirstOrDefault(adv => adv.Id == advertId);

            if (advertDelete != null)
            {
                _operationDb.DeleteAdvert(advertDelete);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ImageModal(Guid advertId)
        {
            var image = _operationDb.GetAdvertisements().FirstOrDefault(adv => adv.Id == advertId).Image;
            return View(image);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
