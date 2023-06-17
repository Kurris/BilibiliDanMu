using LiveCore;
using LiveCore.Interfaces;
using LiveCore.Services;
using Microsoft.AspNetCore.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BarrageServiceCollectionExtensions
    {
        public static IServiceCollection AddLiveBarrage(this IServiceCollection services)
        {
            services.AddSingleton<IBarrageCancellationService, BarrageCancellationService>();
            services.AddTransient<IBarrageService, BarrageService>();

            services.AddLiveBarrageCore();

            return services;
        }

        public static IServiceCollection AddLiveBarrageCore(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddSingleton<BilibiliApiService>()
                    .AddSingleton<EmoteService>()
                    .AddSingleton<AvatarService>()
                    .AddSingleton<GiftService>()
                    .AddSingleton<RawHandleService>();

            services.AddTransient<IBarrageConnectionProvider, BilibiliConnectionService>();

            return services;
        }


        public static IApplicationBuilder UseInternalServiceProvider(this IApplicationBuilder app)
        {
            InternalApp.ApplicationServices = app.ApplicationServices;
            return app;
        }
    }
}
