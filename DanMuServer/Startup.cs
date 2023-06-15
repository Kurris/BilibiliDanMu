
using DanMuServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DanMuServer
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLiveBarrage();
            services.AddSignalR();

            //处理api响应时,循环序列化问题
            //返回json为驼峰命名
            //时间格式
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

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
            //根服务提供器,应用程序唯一
            InternalApp.ApplicationServices = app.ApplicationServices;

            app.UseCors("cors");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BarrageHub>("/barrage", options =>
                {
                    //强制使用ws,不兼容long polling和sse
                    options.Transports = HttpTransportType.WebSockets;
                });

                endpoints.MapControllers();
            });
        }
    }
}
