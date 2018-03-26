using Autofac;
using Gallery.Infrastructure.IoC.Modules;
using Gallery.Infrastructure.Mapper;
using Microsoft.Extensions.Configuration;

namespace Gallery.Infrastructure.IoC
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AutoMapperConfig.Initialize())
                .SingleInstance();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<SqlModule>();
        }
    }
}
