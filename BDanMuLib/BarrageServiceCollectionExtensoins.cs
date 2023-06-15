using System;
using BDanMuLib;
using BDanMuLib.Interfaces;
using BDanMuLib.Services;
using Microsoft.AspNetCore.Builder;

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


        public static IApplicationBuilder UseInternalServiceProvider(this IApplicationBuilder app)
        {
            InternalApp.ApplicationServices = app.ApplicationServices;
            return app;
        }
    }
}
