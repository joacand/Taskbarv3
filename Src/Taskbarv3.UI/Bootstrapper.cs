using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Services;
using Taskbarv3.Infrastructure.DataAccess;
using Taskbarv3.Infrastructure.Services;
using Taskbarv3.UI.Models;
using Unity;
using Unity.Lifetime;

namespace Taskbarv3.UI
{
    public static class Bootstrapper
    {
        public static void AddRegistrations(this IUnityContainer container)
        {
            container.RegisterType<IStatusService, StatusService>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IStatusSetter>(container.Resolve<IStatusService>());
            container.RegisterType<IConfigHandler, ConfigHandler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IShortcutsHandler, ShortcutsHandler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFavoritesService, FavoritesService>();
            container.RegisterType<IHueService, HueService>();
            container.RegisterType<IMaxRequestsGuard, MaxRequestsGuard>();
            container.RegisterType<IMediaControlService, MediaControlService>();
            container.RegisterType<IShortcutsHandler, ShortcutsHandler>();
            container.RegisterType<IWorkAreaService, WorkAreaService>();
            container.RegisterType<ISongViewerService, SongViewerService>();
            container.RegisterType<ICpuUsageService, CpuUsageService>();
            container.RegisterType<IWindowService, WindowService>(new ContainerControlledLifetimeManager());
        }
    }
}
