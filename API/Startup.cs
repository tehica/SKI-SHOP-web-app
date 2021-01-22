using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using AutoMapper;
using API.Helpers;
using API.Middleware;
using API.Extensions;
using StackExchange.Redis;
using Infrastructure.Identity;

// git add .
// git commit -m "message"
// git push origin master


// password for login to all users are: Pa$$w0rd
#region REDIS SERVER INFO
/*
    Redis install
    1.) install and follow 5 steps from this page ( https://chocolatey.org/install ) for install chocolatey
    2.) go to PowerShell, run as Administrator and type: choco install redis-64 --version 3.0.503
    3.) then start Redis server from cmd: redis-server

    when Redis server is run on one cmd windows then open second cmd window and you can type:
    'redis-cli' and then ping server to check is it works

    how to disable redis server:
    1.) redis-cli
    2.) shutdown
*/
#endregion

// for change theme in angular app go to angular.json file and find in styles:[]
//      "./node_modules/bootswatch/dist/Sandstone/bootstrap.min.css"
// and Sandstone word change with other theme name on page: https://bootswatch.com/

// start Angular application : /API/client/ and there type: ng serve

// 102. tutorijal nisam napravio s paginacijom pogledati kasnije jos jednom

// Ctrl + K, Ctrl F

// API namespace = Skinet
/*
 * 
    Host (useful for support):
      Version: 3.1.3
      Commit:  4a9f85e9f8

    .NET Core SDKs installed:
      2.2.207 [C:\Program Files\dotnet\sdk]
      3.1.201 [C:\Program Files\dotnet\sdk]
*/

// 22. 280. nisam pogledao

// Warning
namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // IServiceCollection is extended in Extensions/ApplicationServicesExtensions class
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection")));

            // create new database, thats why we are specifying a separate service for this
            // we have a completely separate database for identity
            // it's gonna be a physical contact boundary between application database (ApplicationDbContext)
            // and the identity database
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(
                    _config.GetConnectionString("IdentityConnection")));

            services.AddControllers();

            // Redis connection is designed to be shared and reuse between callers
            services.AddSingleton<IConnectionMultiplexer>(c => {
                var configuration = ConfigurationOptions.Parse(_config.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            // with this line we access to class where is IServiceCollection extended
            // and inject this class (ApplicationServicesExtensions) and services here
            services.AddApplicationServices();

            // Extensions/IdentityServiceExtensions
            services.AddIdentityServices(_config);

            // this method is define in Extensions/SwaggerServiceExtensions class
            services.AddSwaggerDocumentation();

            // CORS support
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    // thats telling clients application if it's running on an unsecured port we are not going to return any or
                    // we are return a header that's going to allow our browser to display this information
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });

        }
        /*
            IApplicationBuilder is extended in Extensions/SwaggerServiceExtensions class
            for adding swagger middleware
        */
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            // /{0} in this place holder comes http StatusCode
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            // middleware for CORS support
            app.UseCors("CorsPolicy");

            // use authentication in app
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
