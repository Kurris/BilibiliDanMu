using BDanMuLib.Interfaces;
using BDanMuLib.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BarrageServiceCollectionExtensoins
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
                    .AddSingleton<RawtHandleService>();

            services.AddTransient<IBarrageConnectionProvider, BilibiliConnectionService>();

            return services;
        }
    }
}
