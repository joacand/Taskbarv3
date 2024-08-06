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
            var statusService = new StatusService();
            services.AddSingleton<IStatusService>(statusService);
            services.AddSingleton<IStatusSetter>(statusService);
            services.AddSingleton<IConfigHandler, ConfigHandler>();
            services.AddSingleton<IShortcutsHandler, ShortcutsHandler>();
            services.AddSingleton<IWindowService, WindowService>();

            services.AddScoped<IFavoritesService, FavoritesService>();
            services.AddScoped<IHueService, HueService>();
            services.AddScoped<IMaxRequestsGuard, MaxRequestsGuard>();
            services.AddScoped<IMediaControlService, MediaControlService>();
            services.AddScoped<IShortcutsHandler, ShortcutsHandler>();
            services.AddScoped<IWorkAreaService, WorkAreaService>();
            services.AddScoped<ISongViewerService, SongViewerService>();
            services.AddScoped<ICpuUsageService, CpuUsageService>();

            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AddShortcutViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }
    }
}
