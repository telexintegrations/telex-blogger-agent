using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Api.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the error Locally
                _logger.LogError(ex, "An unhandled exception caught in the request middleware.");

                int status = DetermineStatusCode(ex);

                // Return a generic error response to the client
                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";

                var response = JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = status,
                    Title = $"{ex?.Message ?? "An error occurred"}",
                    Detail = ex?.InnerException?.Message ?? ex?.Message,
                    Instance = context.Request.Path, 
                });

                await context.Response.WriteAsync(response);
            }
        }


        private int DetermineStatusCode(Exception exception)
        {
            // Map certain exception types to specifc status codes
            return exception switch
            {
                ArgumentException or ArgumentNullException _ => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException _ => (int)HttpStatusCode.Unauthorized,
                NotImplementedException _ => (int)HttpStatusCode.NotImplemented,
                AuthenticationException _ => (int)HttpStatusCode.Forbidden,
                FileNotFoundException _ => (int)HttpStatusCode.NotFound,

                _ => (int)HttpStatusCode.InternalServerError,

            };
        }
    }
}
