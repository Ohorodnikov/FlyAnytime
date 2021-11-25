using FlyAnytime.Core;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Messaging.Messages.UrlShortener;
using FlyAnytime.Tools;
using FlyAnytime.UrlShortener.EF;
using FlyAnytime.UrlShortener.MessageHandlers;
using FlyAnytime.UrlShortener.Shortener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIEntityAsBase();
            services.AddLazy<ICommonSettings, CommonSettings>(services.AddSingleton);
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<UrlShortenerDbContext>(opt => opt.UseLazyLoadingProxies().UseSqlServer(connection));
            services.AddScoped<IDbContextBase, UrlShortenerDbContext>();

            services.AddTransient<IShortener, Shortener.UrlShortener>();
            services.AddTransient<ILongShortener, LongShortener>();

            services.AddRabbitMq();

            services.AddHttpContextAccessor();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SubscribeOnMessages(app);
        }

        private void SubscribeOnMessages(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

            //eventBus.Subscribe<AppInitMessage, AppInitMessageHandler>();
            //eventBus.Subscribe<ReCreateDbMessage, ReCreateDbMessageHandler>();
            //eventBus.Subscribe<DeleteAllUsersDataMessage, DeleteAllUsersDataHandler>();

            eventBus.Subscribe<GetShortenUrlRequest, GetShortenUrlRequestHandler, GetShortenUrlResponse>();
        }
    }
}
