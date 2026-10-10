using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Application.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories.Users;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Repositories.Tickets;
using Infrastructure.Persistence.Repositories.Categories;
using Infrastructure.Events;
using Infrastructure.Persistence.Repositories.Notifications;
using Infrastructure.Persistence.Repositories.TicketReplies;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ef core
            services.AddDbContext<SupportFlowDbContext>(option => option.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();
            services.AddScoped<ITicketReadRepository, TicketReadRepository>();
            services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
            services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();
            services.AddScoped<INotificationWriteRepository, NotificationWriteRepository>();
            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<ITicketReplyWriteRepository, TicketReplyWriteRepository>();


            services.AddScoped<DatabaseSeeder>();
            services.Configure<SeededUserSettings>(configuration.GetSection(SeededUserSettings.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IPasswordService, BcryptService>();
            services.AddTransient<IJwtService, JsonWebTokenService>();
            services.AddTransient<ITicketNumberGeneratorService, TicketNumberGeneratorService>();
            services.AddTransient<IImageStorageService, CloudinaryService>();

            // Cloudinary
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

                var account = new Account(
                    settings.CloudName,
                    settings.ApiKey,
                    settings.ApiSecret);

                return new Cloudinary(account);
            });

            //event
            services.AddScoped<IEventDispatcher, EventDispatcher>();


            return services;
        }
    }
}
