﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.DataManager;
using BusinessLogic.Options;
using BusinessLogic.Services;
using DataBase.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAppAdverts.Models;

namespace WebAppAdverts.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOperationDb _operationDb;
        private readonly IReCaptchaService _reCaptcha;
        private readonly IConverterService<byte[], IFormFile> _convertImageToBytes;
        private readonly int _countAdvertsByAuftor;
        private readonly int _countAdvertsByPage;

        public HomeController(IOperationDb operationDb, IReCaptchaService reCaptcha, IOptions<AppOptions> options
            ,IConverterService<byte[], IFormFile> convertImageToBytes)
        {
            _operationDb = operationDb;
            _reCaptcha = reCaptcha;
            _countAdvertsByPage = options.Value.IndexOptions.CountAdvertsByPage;
            _countAdvertsByAuftor = options.Value.IndexOptions.CountAdvertsByAuftor;
            _convertImageToBytes = convertImageToBytes;
        }

        [HttpGet]
        public IActionResult Index(string nameSearch = "", string contentSearch = "", int currentPage = 1, SortState sortOrder = SortState.DateDesc)
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
            }

            var count = adverts.Count();
            var items = adverts.Skip((currentPage - 1) * _countAdvertsByPage).Take(_countAdvertsByPage).ToList();

            IndexViewModel indexVM = new IndexViewModel
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
            var user = _operationDb.GetModels<User>().FirstOrDefault(u => u.Name == name);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, name)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

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
        public IActionResult Create()
        {
            var countAdverts = _operationDb.GetModels<Advert>(u => u.User.Name == User.FindFirstValue(ClaimTypes.Name)).Include(u => u.User).Count();

            if (countAdverts < _countAdvertsByAuftor)
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
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

            var userId = _operationDb.GetModels<User>()
                .FirstOrDefault(u => u.Name == User.FindFirstValue(ClaimTypes.Name)).Id;

            Advert advert = new Advert
            {
                Content = createVM.Content,
                Image = _convertImageToBytes.Convert(createVM.Image),
                UserId = userId
            };

            _operationDb.CreateModel(advert);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(Guid advertId)
        {
            var editAdvert = _operationDb.GetModels<Advert>().Include(u => u.User).FirstOrDefault(adv => adv.Id == advertId);

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
        public IActionResult Edit(EditViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editVM);
            }

            Advert advert = _operationDb.GetModels<Advert>().Include(u => u.User).FirstOrDefault(adv => adv.Id == editVM.AdvertId);
            advert.Content = editVM.Content;
            advert.DateTime = DateTime.Now;

            if (editVM.Image != null)
            {
                advert.Image = _convertImageToBytes.Convert(editVM.Image);
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
        public IActionResult DeleteConfirmed(Guid advertId)
        {
            var advertDelete = _operationDb.GetModels<Advert>().FirstOrDefault(adv => adv.Id == advertId);

            if (advertDelete != null)
            {
                _operationDb.RemoveModel(advertDelete);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
