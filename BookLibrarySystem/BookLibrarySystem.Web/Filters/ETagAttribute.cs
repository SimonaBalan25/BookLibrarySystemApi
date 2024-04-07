using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace BookLibrarySystem.Web.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class ETagAttribute : Attribute, IActionFilter
    {
        private static string currentETag;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after the action execution
            // Retrieve client's ETag from request headers
            var clientETag = context.HttpContext.Request.Headers["If-None-Match"];
            var responseObj = ((ObjectResult)context.Result).Value;
            // Generate or retrieve the current ETag for the resource
            string currentETag = GenerateETag(responseObj);


            // Check if the resource is modified
            bool isModified = CheckIfResourceIsModified(clientETag, currentETag);

            // If the resource is not modified, return 304
            if (!isModified)
            {
                context.Result = new StatusCodeResult(304);
            }

            // Set the ETag header in the response
            context.HttpContext.Response.Headers.Add("ETag", currentETag);
        }

        private bool CheckIfResourceIsModified(string clientETag, string currentETag)
        {
            // If client sent an ETag and it matches the current ETag, resource is not modified
            return clientETag != currentETag;
        }

        private string GenerateETag(object value)
        {
            // Generate an ETag based on the current state of the resource
            // For demonstration, using a simple hash of a unique identifier
            var uniqueIdentifier = Guid.NewGuid().ToString();
            using (var md5 = MD5.Create())
            {
                var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
                byte[] hashBytes = md5.ComputeHash(jsonBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}