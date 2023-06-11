using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DanMuServer
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("cors", builder =>
                {
                    builder.SetIsOriginAllowed(x => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("cors");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DanMuHub>("/danmu");
            });
        }
    }
}
