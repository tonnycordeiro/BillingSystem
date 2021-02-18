using Company.BillingSystem.ReportApi.Models;
using Company.BillingSystem.ReportApi.Providers;
using Company.BillingSystem.ReportApi.Services;
using Company.BillingSystem.ReportApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi
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
            ConfigProvider configProvider = new ConfigProvider();

            services.Configure<ReportGeneratorSettings>(
                configProvider.Configuration.GetSection(nameof(ReportGeneratorSettings)));

            services.AddSingleton<IReportGeneratorSettings>(
                sp => sp.GetRequiredService<IOptions<ReportGeneratorSettings>>().Value);

            services.AddSingleton<IBillingApiService>(
                sp => new BillingApiService(sp.GetRequiredService<IHttpClientFactory>()));

            services.AddSingleton<ICustomerApiService>(
                sp => new CustomerApiService(sp.GetRequiredService<IHttpClientFactory>()));

            services.AddHttpClient(CustomerApiSettings.HTTP_CLIENT_NAME, c =>
            {
                c.BaseAddress = new Uri(configProvider.CustomerApiSettings.BaseAddress);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddHttpClient(BillingApiSettings.HTTP_CLIENT_NAME, c =>
            {
                c.BaseAddress = new Uri(configProvider.BillingApiSettings.BaseAddress);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CandidateTesting.BillingSystem.ReportApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CandidateTesting.BillingSystem.ReportApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
