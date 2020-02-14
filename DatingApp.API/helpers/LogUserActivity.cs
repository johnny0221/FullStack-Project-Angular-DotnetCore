using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // allow us to execute some code before or after the action, it's like dotnet Core interceptor.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // this variable will allow us to have access to things like HttpContext.
            var resultContext = await next();

            // the code below will be executed after the action has been completed.

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // services are also available in the HttpContext
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();

            var user = await repo.GetUser(userId);

            user.LastActive = DateTime.Now;

            await repo.SaveAll();

        }
    }
}