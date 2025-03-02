﻿using Microsoft.Extensions.Caching.Memory;
using ShopHub.API.Helper;
using System.Net;
using System.Text.Json;

namespace ShopHub.API.Middleware
{
    public class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);
        public ExceptionsMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurity(context);
                if (IsRequestAllowed(context) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                    var response = new ApiExceptions((int)HttpStatusCode.TooManyRequests, "Too many requests, Please try again later");
                    await context.Response.WriteAsJsonAsync(response);
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var response = _environment.IsDevelopment() ?
                    new ApiExceptions((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
                    : new ApiExceptions((int)HttpStatusCode.InternalServerError, ex.Message);

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

        private bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress;
            var cachKey = $"Rate:{ip}";
            var dateNow = DateTime.Now;

            var (timeStamp, count) = _memoryCache.GetOrCreate(cachKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (timeStamp: dateNow, count: 0);
            });
            if (dateNow - timeStamp < _rateLimitWindow)
            {
                if (count >= 8)
                {
                    return false;
                }
                _memoryCache.Set(cachKey, (timeStamp, count += 1), _rateLimitWindow);

            }
            else
            {
                _memoryCache.Set(cachKey, (timeStamp, count), _rateLimitWindow);
            }
            return true;
        }

        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1;mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }
    }
}
