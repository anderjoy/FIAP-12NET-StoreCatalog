﻿using AutoMapper;
using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.Repository;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using GeekBurger.StoreCatalog.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Net.Http;

namespace StoreCatalog.WebAPI
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
            services.AddDbContext<StoreContext>();
            services.AddSingleton(f => new HttpClient());

            services.AddScoped<IProductionAreaService, ProductionAreaService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductionAreaRepository, ProductionAreaRepository>();
            
            services.AddScoped<ISendMessageServiceBus, SendMessageServiceBus>();
            services.AddScoped<IReceiveMessageServiceBus, ReceiveMessageServiceBus>();

            services.AddScoped<IStoreCatalogInitialization, StoreCatalogInitialization>();

            services.AddSingleton<ILogServiceBus, LogServiceBus>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "StoreCatalog API", Version = "v1" });
            });

            services.AddAutoMapper();

            var mvcCoreBuilder = services.AddMvcCore();

            mvcCoreBuilder
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddCors();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreCatalog API");
            });

            app.UseMvc();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var storeCatalogInitialize = scope.ServiceProvider.GetService<IStoreCatalogInitialization>();
                await storeCatalogInitialize.InitializeStoreCatalog();

                var receiveMessageServiceBus = scope.ServiceProvider.GetService<IReceiveMessageServiceBus>();                
            }            
        }
    }
}
