using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    // with this class we extend IServiceCollection what is passed as parameter in Startup.cs
    // ConfigureServices method
    public static class ApplicationServicesExtensions
    {
        // (this IServiceCollection) is passed as parameter because we want extend this class
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

            // token
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IOrderService, OrderService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    // check if any errors is in ModelState and if errors count > 0
                    // then select them and add into array
                    var errors = actionContext.ModelState
                                              .Where(e => e.Value.Errors.Count > 0)
                                              .SelectMany(x => x.Value.Errors)
                                              .Select(x => x.ErrorMessage).ToArray();

                    // populate Errors property in ApiValidationErrorResponse with 'errors' <- array
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    // errorResponse is ApiValidationErrorResponse and it is passed as request
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}
