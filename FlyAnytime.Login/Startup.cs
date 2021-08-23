using FlyAnytime.Core.Entity;
using FlyAnytime.Login.EF;
using FlyAnytime.Login.Helpers;
using FlyAnytime.Login.JWT;
using FlyAnytime.Login.MessageHandlers;
using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Helpers;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlyAnytime.Login
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
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<LoginContext>(opt => opt.UseLazyLoadingProxies().UseSqlServer(connection));

            services.SetCommonJwtSettings();

            services.AddScoped<ITokenBuilder, TokenBuilder>();

            services.AddIEntityAsBase();
            services.AddTransient<IOclHelper, OclHelper>();

            services.AddRabbitMq();

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

            eventBus.Subscribe<GetLoginLinkRequestMessage, GetLoginLinkHandler, GetLoginLinkResponseMessage>();
        }
    }
}
