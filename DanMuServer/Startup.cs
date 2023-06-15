
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

            //����api��Ӧʱ,ѭ�����л�����
            //����jsonΪ�շ�����
            //ʱ���ʽ
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
            //�������ṩ��,Ӧ�ó���Ψһ
            InternalApp.ApplicationServices = app.ApplicationServices;

            app.UseCors("cors");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BarrageHub>("/barrage", options =>
                {
                    //ǿ��ʹ��ws,������long polling��sse
                    options.Transports = HttpTransportType.WebSockets;
                });

                endpoints.MapControllers();
            });
        }
    }
}
