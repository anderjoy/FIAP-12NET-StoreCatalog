using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using GeekBurger.StoreCatalog.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreCatalog.WebAPI.Models;
using Swashbuckle.AspNetCore.Swagger;

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

            services.AddTransient<IProductionAreaService, ProductionAreaService>();
            services.AddTransient<IProductService, ProductService>();

            services.AddSingleton<IStoreCatalogInitialization, StoreCatalogInitialization>();

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            var storeCatalogInitialize = app.ApplicationServices.GetService<IStoreCatalogInitialization>();
        }
    }
}
