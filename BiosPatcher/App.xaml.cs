using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using BiosPatcher.View;

namespace BiosPatcher
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var container = InitializeContainer();
            var patchingWindow = container.Resolve<PatchingWindow>();
            patchingWindow.Show();
        }

        private static IContainer InitializeContainer()
        {
            var types = typeof(App).Assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<ComponentAttribute>() != null);
            var containerBuilder = new ContainerBuilder();
            foreach (var type in types)
            {
                containerBuilder.RegisterType(type).AsImplementedInterfaces();
            }

            containerBuilder.RegisterType<PatchingWindow>().AsSelf();
            return containerBuilder.Build();
        }
    }
}