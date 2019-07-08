using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.DataManager;
using BusinessLogic.Options;
using DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Filters
{
    public class AdvertCountByAuthorFilter : Attribute, IAsyncResourceFilter
    {
        private readonly IOperationDb _db;
        private readonly int _advertsCountByAuthor;
        
        public AdvertCountByAuthorFilter(IOperationDb db, IOptions<AppOptions> options)
        {
            _db = db;
            _advertsCountByAuthor = options.Value.IndexOptions.CountAdvertsByAuthor;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var userId = new Guid(context.HttpContext.User.FindFirstValue("UserId"));
            var count = await _db.GetModels<Advert>(adv => adv.UserId == userId).CountAsync();

            if (!(count < _advertsCountByAuthor))
            {
                context.Result = new RedirectResult("~/Home/Index");
                await context.Result.ExecuteResultAsync(context);
            }
            else
            {
                await next();
            }
        }
    }
}