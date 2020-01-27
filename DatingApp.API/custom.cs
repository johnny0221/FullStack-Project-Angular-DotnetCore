using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API
{
    public class custom
    {
        private readonly RequestDelegate _next;

        public custom(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("hello world");
            await _next(context);
        }
    }

    public static class customExtensions
    {
        public static IApplicationBuilder UseCustom(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<custom>();

        }
    }
}