﻿using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web.Entities;
using Newtonsoft.Json;
using NLog.Fluent;
using System;
using System.Net;

namespace BookLibrarySystem.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var errorMessage = string.Empty;
#if DEBUG
            errorMessage = JsonConvert.SerializeObject(new ErrorDetails { 
                Message = ex.Message, 
                StatusCode = context.Response.StatusCode, 
                StackTrace = ex.StackTrace, 
                Source= ex.TargetSite?.DeclaringType?.FullName, 
                ErrorId=Guid.NewGuid().ToString()
            });
#else
            errorMessage = JsonConvert.SerializeObject(new ErrorDetails { 
                Message = "An error occurred. Please try again later.", 
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorId = Guid.NewGuid().ToString() });
#endif

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(errorMessage);
            }
            else
            {
                _logger.LogInformation("Can't write error response. Response has already started.");
            }
        }
    }
}
