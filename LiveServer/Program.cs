using LiveServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog();

}).UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
.ConfigureServices(services =>
{
    services.AddLiveBarrage();
    services.AddSignalR();

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
});


var app = builder.Build();
app.UseInternalServiceProvider();
app.UseCors("cors");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<BarrageHub>("/barrage", options =>
    {
        options.Transports = HttpTransportType.WebSockets;
    });
});


app.Run();
