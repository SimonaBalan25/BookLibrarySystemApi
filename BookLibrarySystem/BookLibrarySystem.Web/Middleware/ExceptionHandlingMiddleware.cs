using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Web.Entities;
using Microsoft.ApplicationInsights;
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
        private readonly TelemetryClient _aiLogger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger, TelemetryClient aiLogger)
        {
            this.requestDelegate = requestDelegate;
            _logger = logger;
            _aiLogger = aiLogger;
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
                _logger.LogError(errorMessage);
                _aiLogger.TrackTrace(errorMessage);
                await context.Response.WriteAsync(errorMessage);
            }
            else
            {
                var message = "Can't write error response. Response has already started.";
                _logger.LogInformation(message);
                _aiLogger.TrackTrace(message);
            }
        }
    }
}
