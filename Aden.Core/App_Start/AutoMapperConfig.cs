using AutoMapper;
using System.Reflection;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Aden.Core.AutoMapperConfig), "Configure")]
namespace Aden.Core
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {

            //HeroicAutoMapperConfigurator.LoadMapsFromCallerAndReferencedAssemblies(x => x.Name.StartsWith("YourPrefix"));
            //HeroicAutoMapperConfigurator.LoadMapsFromCallerAndReferencedAssemblies();
            //If you run into issues with the maps not being located at runtime, try using this method instead: 
            //HeroicAutoMapperConfigurator.LoadMapsFromAssemblyContainingTypeAndReferencedAssemblies<SomeControllerOrTypeInYourWebProject>();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
            });

        }



    }
}