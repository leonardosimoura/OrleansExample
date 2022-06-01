using Orleans;
using OrleansExampleGrainsInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseOrleansClient((builder) =>
{
    builder.UseLocalhostClustering();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/setup", async (IGrainFactory grainFactory) =>
{
    var executions = Enumerable.Range(1, 100);
    await Parallel.ForEachAsync(executions, async (execution,_) =>
    {
        var deviceId = execution.ToString();
        var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);
        var deviceInfo = new DeviceInfo($"Device - {deviceId}", new List<ILightGrain>());
        await device.SetInfo(deviceInfo);

    });
  
    return true;
})
.WithName("Setup");

app.MapGet("/turn-on", async (IGrainFactory grainFactory) =>
{
    var executions = Enumerable.Range(1, 100);
    await Parallel.ForEachAsync(executions, async (execution,_) =>
    {
        var deviceId = execution.ToString();
        var device = grainFactory.GetGrain<IDeviceGrain>(deviceId);

        for (int i = 1; i <= 100; i++)
        {
            var lightId = deviceId + "/" + i;

            var light = grainFactory.GetGrain<ILightGrain>(lightId);

            var lightInfo = await light.Info();
            if (lightInfo == null)
                await light.SetInfo(new LightInfo($"Light - {lightId}", false));

            await device.TurnLightOn(light);

            var deviceInfo = await device.Info();
        }
    });
  
    return true;
})
.WithName("TurnOn");
app.Run();
