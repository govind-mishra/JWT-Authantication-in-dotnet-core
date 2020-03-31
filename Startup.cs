using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiUsingCore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestApiUsingCore.Options;
using RestApiUsingCore.Installer;

namespace RestApiUsingCore
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
            //you can write the complete below line or make the extension method for this
            
            #region for writing in multiple lines

           /* var alltheClassesimplementingInstaller = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            alltheClassesimplementingInstaller.ForEach(x => x.InstallServices(Configuration,services));*/

            #endregion

            //or using extension mehtod of service to use single line of code
            services.InstallerServiceinAssembly(Configuration);


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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            var swaggerOption = new SwaggerOptions();
            //below line means that variable swaggerOption defined here is the binder in appseting.json and will bind with SwaggeOption class
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOption);
            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOption.JsonRoute;
            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOption.UIEndPoint, swaggerOption.Description);
            });

            app.UseMvc();
        }
    }
}
