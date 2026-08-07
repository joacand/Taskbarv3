using Microsoft.Extensions.DependencyInjection;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Services;
using Taskbarv3.Infrastructure.DataAccess;
using Taskbarv3.Infrastructure.Services;
using Taskbarv3.UI.Models;
using Taskbarv3.UI.ViewModels;

namespace Taskbarv3.UI.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddRegistrations(this IServiceCollection services)
        {
            services.AddSingleton<StatusService>();
            services.AddSingleton<IStatusService>(sp => sp.GetRequiredService<StatusService>());
            services.AddSingleton<IStatusSetter>(sp => sp.GetRequiredService<StatusService>());
            services.AddSingleton<IConfigHandler, ConfigHandler>();
            services.AddSingleton<IShortcutsHandler, ShortcutsHandler>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IFavoritesService, FavoritesService>();
            services.AddSingleton<IHueService, HueService>();
            services.AddSingleton<IMediaControlService, MediaControlService>();
            services.AddSingleton<IWorkAreaService, WorkAreaService>();
            services.AddSingleton<ISongViewerService, SongViewerService>();
            services.AddSingleton<ICpuUsageService, CpuUsageService>();

            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AddShortcutViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }
    }
}
