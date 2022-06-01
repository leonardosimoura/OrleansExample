using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using OrleansExampleGrains;
using OrleansExampleGrainsInterfaces;

;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Orleans Example", Description = "", Version = "v1" });
});




builder.Host.UseOrleans((builder) =>
{
    builder.UseLocalhostClustering()
    .AddMemoryGrainStorageAsDefault()
    .UseInMemoryReminderService()
    .AddActivityPropagation();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Orleans Example v1");
    });
}

app.UseHttpsRedirection();


app.MapGet("/cluster-info", async (IGrainFactory grainFactory) =>
{
    var managementGrain = grainFactory.GetGrain<IManagementGrain>(0);

    var ss = await managementGrain.GetSimpleGrainStatistics();
    var statistics = await managementGrain.GetDetailedGrainStatistics();
    return ss.GroupBy(g => g.GrainType)
    .Select(s => new
    {
        Type = s.Key.Split(",").First(),
        ActivationCount = s.Sum(v => v.ActivationCount),
    })
    .OrderByDescending(o => o.ActivationCount);
}).WithName("ClusterInfo");

app.MapGet("/setup/{from}/{to}", async (int from, int to ,IGrainFactory grainFactory) =>
{
    var executions = Enumerable.Range(from, to);
    await Parallel.ForEachAsync(executions, async (execution, _) =>
    {
        var deviceId = execution.ToString();
        var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);
        var deviceInfo = new DeviceInfo($"Device - {deviceId}");
        await device.SetInfo(deviceInfo);

    });

    return true;
})
.WithName("Setup");

app.MapGet("/device/{deviceId}", async (string deviceId, IGrainFactory grainFactory) =>
{
    var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);
    return await device.InfoDetail();
})
.WithName("Device");

app.MapGet("/turn-light-on/{from}/{to}", async (int from, int to,IGrainFactory grainFactory) =>
{
    var executions = Enumerable.Range(from, to);
    await Parallel.ForEachAsync(executions, async (execution, _) =>
    {
        var deviceId = execution.ToString();
        var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);

        for (int i = 1; i <= 10; i++)
        {
            var lightId = deviceId + "/" + i;

            var light = grainFactory.GetGrain<ILightGrain>(lightId);

            var lightInfo = await light.Info();
            if (lightInfo == null)
                await light.SetInfo(new LightInfo($"Light - {lightId}", false));

            await device.TurnLightOn(light);
        }
    });

    return true;
})
.WithName("TurnLightOn");

app.MapGet("/turn-light-off/{from}/{to}", async (int from, int to,IGrainFactory grainFactory) =>
{
    var executions = Enumerable.Range(from, to);
    await Parallel.ForEachAsync(executions, async (execution, _) =>
    {
        var deviceId = execution.ToString();
        var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);

        for (int i = 1; i <= 10; i++)
        {
            var lightId = deviceId + "/" + i;

            var light = grainFactory.GetGrain<ILightGrain>(lightId);

            var lightInfo = await light.Info();
            if (lightInfo == null)
                await light.SetInfo(new LightInfo($"Light - {lightId}", false));

            await device.TurnLightOff(light);
        }
    });

    return true;
})
.WithName("TurnLightOff");

app.Run();