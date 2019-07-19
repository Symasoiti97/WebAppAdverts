using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using BusinessLogic.DataManager;
using BusinessLogic.Filters;
using BusinessLogic.Options;
using BusinessLogic.Services;
using DataBase.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAppAdverts.Models;
using Z.EntityFramework.Plus;

namespace WebAppAdverts.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOperationDb _operationDb;
        private readonly IReCaptchaService _reCaptcha;
        private readonly IImageService _imageService;
        private readonly int _countAdvertsByPage;

        public HomeController(IOperationDb operationDb, IReCaptchaService reCaptcha, IOptions<AppOptions> options
            , IImageService imageService)
        {
            _operationDb = operationDb;
            _reCaptcha = reCaptcha;
            _countAdvertsByPage = options.Value.IndexOptions.CountAdvertsByPage;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string nameSearch = "", string contentSearch = "", int currentPage = 1,
            SortState sortOrder = SortState.DateDesc)
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
            var items = await adverts.Skip((currentPage - 1) * _countAdvertsByPage).Take(_countAdvertsByPage)
                .ToListAsync();

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

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

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

            var urlImage = await _imageService.WriteImageAndGetPathAsync(createVM.Image);

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

            var urlImage = editVM.Image != null
                ? await _imageService.WriteImageAndGetPathAsync(editVM.Image)
                : editVM.ImageUrl;

            await _operationDb.GetModels<Advert>(adv => adv.Id == editVM.AdvertId)
                .UpdateAsync(adv => new Advert()
                {
                    Content = editVM.Content,
                    DateTime = DateTime.Now,
                    Image = urlImage
                });

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
            await _operationDb.GetModels<Advert>(adv => adv.Id == advertId).DeleteAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("/Home/UpRating/{advertId}")]
        public async Task<JsonResult> UpRating(Guid advertId)
        {
            var transactionOptions = new TransactionOptions() {IsolationLevel = IsolationLevel.RepeatableRead};
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _operationDb.GetModels<Advert>(adv => adv.Id == advertId)
                    .UpdateAsync(adv => new Advert() {Rating = adv.Rating + 1});

                var advert = await _operationDb.GetModels<Advert>().FirstOrDefaultAsync(adv => adv.Id == advertId);

                transaction.Complete();

                return Json(advert.Rating);
            }
        }

        [HttpGet]
        [Route("/Home/DownRating/{advertId}")]
        public async Task<JsonResult> DownRating(Guid advertId)
        {
            var transactionOptions = new TransactionOptions() {IsolationLevel = IsolationLevel.RepeatableRead};
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await _operationDb.GetModels<Advert>(adv => adv.Id == advertId)
                    .UpdateAsync(adv => new Advert() {Rating = adv.Rating - 1});

                var advert = await _operationDb.GetModels<Advert>().FirstOrDefaultAsync(adv => adv.Id == advertId);

                transaction.Complete();

                return Json(advert.Rating);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
