using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Installer
{
    public static class InstallerExtensions
    {
        public static void InstallerServiceinAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var alltheClassesimplementingInstaller = typeof(Startup).Assembly.ExportedTypes
               .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
               .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            alltheClassesimplementingInstaller.ForEach(x => x.InstallServices(configuration, services));
        }
    }
}
