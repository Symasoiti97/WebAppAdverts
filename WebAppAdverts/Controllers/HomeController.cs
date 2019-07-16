using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.DataManager;
using BusinessLogic.Filters;
using BusinessLogic.Options;
using BusinessLogic.Services;
using DataBase.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WebAppAdverts.Models;

namespace WebAppAdverts.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOperationDb _operationDb;
        private readonly IReCaptchaService _reCaptcha;
        private readonly IWriteImageService _writeImageService;
        private readonly int _countAdvertsByPage;
        private readonly string _imagesPath;

        public HomeController(IOperationDb operationDb, IReCaptchaService reCaptcha, IOptions<AppOptions> options
            , IWriteImageService writeImageService)
        {
            _operationDb = operationDb;
            _reCaptcha = reCaptcha;
            _countAdvertsByPage = options.Value.IndexOptions.CountAdvertsByPage;
            _writeImageService = writeImageService;
            _imagesPath = options.Value.ImagesPath.Path;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string nameSearch = "", string contentSearch = "", int currentPage = 1, SortState sortOrder = SortState.DateDesc)
        {
            IQueryable<Advert> adverts = _operationDb.GetModels<Advert>().Include(u => u.User);

            if (!String.IsNullOrEmpty(nameSearch))
            {
                adverts = adverts.Where(adv => adv.User.Name.Contains(nameSearch));
            }

            if (!String.IsNullOrEmpty(contentSearch))
            {
                adverts = adverts.Where(adv => adv.Content.Contains(contentSearch));
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
                case SortState.NumberDesc:
                    adverts = adverts.OrderByDescending(sort => sort.Number);
                    break;
                case SortState.NumberAsc:
                    adverts = adverts.OrderBy(sort => sort.Number);
                    break;
            }

            var count = await adverts.CountAsync();
            var items = await adverts.Skip((currentPage - 1) * _countAdvertsByPage).Take(_countAdvertsByPage).ToListAsync();

            var indexVM = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, currentPage, _countAdvertsByPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(nameSearch, contentSearch),
                Adverts = items
            };

            return View(indexVM);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string name)
        {
            var user = await _operationDb.GetModels<User>().FirstOrDefaultAsync(u => u.Name == name);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim("UserId", user.Id.ToString())
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        [TypeFilter(typeof(AdvertCountByAuthorFilter))]
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

            var urlImage = await _writeImageService.WriteImageAndGetPathAsync(createVM.Image);
            
            var advert = new Advert
            {
                Content = createVM.Content,
                Image = urlImage,
                UserId = new Guid(User.FindFirstValue("UserId"))
            };

            _operationDb.CreateModel(advert);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid advertId)
        {
            var editAdvert = await _operationDb.GetModels<Advert>().FirstOrDefaultAsync(adv => adv.Id == advertId);

            var advertVM = new EditViewModel
            {
                AdvertId = editAdvert.Id,
                Content = editAdvert.Content,
                ImageUrl = editAdvert.Image
            };

            return View(advertVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editVM);
            }

            var advert = await _operationDb.GetModels<Advert>().FirstOrDefaultAsync(adv => adv.Id == editVM.AdvertId);
            advert.Content = editVM.Content;
            advert.DateTime = DateTime.Now;
            
            

            if (editVM.Image != null)
            {
                var urlImage = await _writeImageService.WriteImageAndGetPathAsync(editVM.Image);

                advert.Image = urlImage;
            }

            _operationDb.UpdateModel(advert);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public IActionResult DeleteModal(Guid advertId)
        {
            return View(advertId);
        }

        [HttpPost, ActionName(nameof(DeleteModal))]
        public async Task<IActionResult> DeleteConfirmed(Guid advertId)
        {
            var advertDelete = await _operationDb.GetModels<Advert>().FirstOrDefaultAsync(adv => adv.Id == advertId);

            if (advertDelete != null)
            {
                _operationDb.RemoveModel(advertDelete);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("/images/{image}")]
        public IActionResult Img(string image, int width, int height)
        {
            var imagePath = _imagesPath + image;
            
            using (var img = Image.Load(imagePath))
            {
                if (width <= 0)
                {
                    width = img.Width;
                }

                if (height <= 0)
                {
                    height = img.Height;
                }
                
                var newImagePath = imagePath.Replace(".", $"_{width}_{height}.");

                img.Mutate(x => x
                    .Resize(width, height));
                
                img.Save(newImagePath);
                var im = System.IO.File.OpenRead(newImagePath);
            
                return File(im, "image/jpeg");
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
