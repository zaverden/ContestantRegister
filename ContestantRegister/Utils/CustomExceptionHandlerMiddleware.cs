using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ContestantRegister
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (EntityNotFoundException e)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }
            catch
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
