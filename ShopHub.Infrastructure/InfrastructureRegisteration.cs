using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ShopHub.Core.Interfaces;
using ShopHub.Core.Services;
using ShopHub.Infrastructure.Data;
using ShopHub.Infrastructure.Repositories;
using ShopHub.Infrastructure.Repositories.Service;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, 
            IConfiguration configuration)
        {
            // when i do not have save process
            // services.AddTransient

            // once all program life time
            // services.AddSingleton

            // have save temp in memory
            // services.AddScoped

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // services.AddScoped<ICategoryRepository, CategoryRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped<IPhotoRepository, PhotoRepository>();
            
            // apply unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // register email sender
            services.AddScoped<IEmailService, EmailService>();

            // apply redis connection
            services.AddSingleton<IConnectionMultiplexer>(i =>
            {
                var config = ConfigurationOptions
                .Parse(configuration.GetConnectionString("redis"));

                return ConnectionMultiplexer.Connect(config);
            });

            services.AddSingleton<IImageManagementService, ImageManagementService>();
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            // apply dbcontext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ShopHubDb"));
            });

            return services;
        }
    }
}
