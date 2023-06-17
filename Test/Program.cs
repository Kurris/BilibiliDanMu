using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using LiveCore.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;

var serviceProvider = new ServiceCollection()
    .AddLogging((builder) =>
    {
        var configuration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", true, true)
         .Build();

        builder.AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger());
    })
    .AddLiveBarrageCore()
    .BuildServiceProvider();

var logger = serviceProvider.GetService<ILogger<Program>>()!;


var roomIds = new[] { 732, 6750632, 23423267, 8604981, 3331090, 888, 25836285, 5558, 1209, 531, 1220, 178033, 404, 22472556 };

var currentRoomIds = roomIds.ToList();
var cancelSources = currentRoomIds.ToDictionary(x => x, x => new CancellationTokenSource());

var excludes = new List<int>(currentRoomIds.Count);

cancelSources.Distinct().AsParallel().ForAll(async entry =>
{
    var roomId = entry.Key;
    var cancelSource = entry.Value;

    await using var provider = serviceProvider.GetService<IBarrageConnectionProvider>()!;
    var connectState = await provider.ConnectAsync(roomId, result =>
    {
        logger.LogInformation("{Type}:{Info}", result.Type, JsonConvert.SerializeObject(result.Info));

    }, cancelSource.Token);

    if (connectState)
    {
        excludes.Add(roomId);
    }

});

cancelSources.Where(x => !excludes.Contains(x.Key)).ToList().ForEach(async entry =>
{
    var millisecond = (currentRoomIds.IndexOf(entry.Key) + 1) * new Random().Next(1, currentRoomIds.Count + 1) * 1000;
    await Task.Delay(millisecond);
    //entry.Value.CancelAfter(millisecond);
    entry.Value.Cancel();
});

while (!cancelSources.Where(x => !excludes.Contains(x.Key)).All(x => x.Value.IsCancellationRequested))
{
    await Task.Delay(1000);
}


logger.LogInformation("All done now, press any key to exit the program !");
Console.ReadKey();