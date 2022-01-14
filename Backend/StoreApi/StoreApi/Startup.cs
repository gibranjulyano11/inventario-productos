using FluentValidation.AspNetCore;
using Lib.Service.Mongo;
using Lib.Service.Mongo.Context;
using Lib.Service.Mongo.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StoreApi.Core.Application.AttributeLogic;
using StoreApi.Core.Application.ProductLogic;

namespace StoreApi
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

            services.AddControllers()
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductCreate>());
            services.AddMediatR(typeof(ProductCreate.ProductCreateCommand).Assembly)
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<AttributeCreate>());
            services.AddMediatR(typeof(AttributeCreate.AttributeCreateCommand).Assembly);

            services.Configure<MongoContext>(opt =>
            {
                opt.ConnectionString = Configuration.GetConnectionString("MongoDB");
                opt.Database = Configuration.GetConnectionString("Database");
            });

            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StoreApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}