using System.Reflection;
using Autofac;
using Gallery.Infrastructure.Services;
using Module = Autofac.Module;

namespace Gallery.Infrastructure.IoC.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(ServiceModule)
                .GetTypeInfo()
                .Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.IsAssignableTo<IService>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EncryptionService>()
                .As<IEncryptionService>()
                .SingleInstance();

            builder.RegisterType<TokenStoreService>()
                .As<ITokenStoreService>()
                .SingleInstance();
        }
    }
}
