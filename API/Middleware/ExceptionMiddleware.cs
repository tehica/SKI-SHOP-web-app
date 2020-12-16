using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // this means if there is no expection then the request moves on to its next stage
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                // set StatusCode to be a five hundred internal server
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // check to see if we are in development env
                /*
                    if we are in Development env execute from '?'

                    or if we are in Production env execute from ':'
                */
                var response = _env.IsDevelopment()
                               ? new ApiException((int)HttpStatusCode.InternalServerError, 
                                                   ex.Message, 
                                                   ex.StackTrace.ToString())

                               : new ApiException((int)HttpStatusCode.InternalServerError, 
                                                   ex.Message, 
                                                   ex.StackTrace.ToString());

                // this options ensure that Json respond returns as camelCase
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
