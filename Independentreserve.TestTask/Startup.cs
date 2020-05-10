using System;
using System.Text.Json.Serialization;
using IndependentReserve.DataProviders;
using IndependentReserve.Worker.Worker;
using IndependentReserve.Worker.Worker.ConfigurationsModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IndependentReserve.TestTaskService
{
    public class Startup
    {

        private OrderBookWorker _orderBookWorker;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(Configuration);
            services.AddSingleton(provider =>
            {
                _orderBookWorker = new OrderBookWorker(new ProviderConfigModel
                {
                    IndependentReserveApiUri = new Uri(Configuration["IndependentReserveApiUri"]),
                    WebSocketChannelUri = new Uri(Configuration["WebSocketChannelUri"])
                });
                _orderBookWorker.Start();
                return _orderBookWorker;
            });

            services.AddTransient<OrderBookProvider>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        }

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

            var applicationLifetime = app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

        }

        private void OnShutdown()
        {
            _orderBookWorker?.ShutDown();
        }

    }
}
