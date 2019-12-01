using CustomerService.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CustomerService.Options;
using CustomerService.Services.Interfaces;

namespace CustomerService
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
            services.AddDbContext<CustomersDBContext>(options => options.UseInMemoryDatabase(databaseName: "Customers"));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<CustomersDBContext>();
            services.AddScoped<ICustomerService, Services.CustomerService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { 
                    Title = "Customer Service API", 
                    Version = "v1",
                    Description = "Customer Service API (ASP.NET Core 2.2)", 
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact()
                    {
                        Name = "Nirasha Gunasekera",
                        Email = "nirasha_pr@yahoo.com"
                    }
                });
            });            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOptions.JsonRoute;
            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);   
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();           

            app.UseMvc();
        }
    }
}
