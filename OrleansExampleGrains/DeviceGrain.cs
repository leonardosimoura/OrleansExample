using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Placement;
using Orleans.Runtime;
using OrleansExampleGrainsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansExampleGrains
{
    public class DeviceGrain : Grain, IDeviceGrain
    {
        const string LIGHT_OFF_REMINDER = "Reminder_Lights_Off";
        private DeviceInfo _deviceInfo;
        private ILogger<DeviceGrain> _logger { get; }

        public DeviceGrain(ILogger<DeviceGrain> logger)
        {
            _deviceInfo = new DeviceInfo("Not Activated");
            _deviceInfo.SetId(base.GrainReference.GetPrimaryKeyString());
            _logger = logger;
        }

        public Task<string> Name()
        {
            return Task.FromResult(_deviceInfo!.Name);
        }

        public Task SetInfo(DeviceInfo info)
        {
            _deviceInfo = info;
            _deviceInfo.SetId(base.GrainReference.GetPrimaryKeyString());
            return Task.CompletedTask;
        }

        private void TryAddLight(string lightId)
        {
            if (!_deviceInfo.Lights.Any(a => a == lightId))
                _deviceInfo.Lights.Add(lightId);
        }

        public async Task TurnLightOff(ILightGrain light)
        {
            await light.TurnLightOff();
            TryAddLight(light.GetPrimaryKeyString());
        }

        public async Task TurnLightOn(ILightGrain light)
        {
            await light.TurnLightOn();
            TryAddLight(light.GetPrimaryKeyString());
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            RegisterOrUpdateReminder(LIGHT_OFF_REMINDER, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1));

            return base.OnActivateAsync(cancellationToken);
        }

        public async Task<DeviceInfoDetail> InfoDetail()
        {
            var detail = new DeviceInfoDetail()
            {
                Id = _deviceInfo.Id,
                Name = _deviceInfo.Name
            };

            foreach (var lightId in _deviceInfo.Lights)
            {
                var ligth = base.GrainFactory.GetGrain<ILightGrain>(lightId);
                detail.Lights.Add(await ligth.Info());
            }

            return detail;
        }

        public Task<DeviceInfo> Info()
        {
            return Task.FromResult(_deviceInfo);
        }


        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            _logger.LogInformation($"Device - {base.GrainReference.GetPrimaryKeyString()} - Running Reminder {reminderName}");
            if (reminderName == LIGHT_OFF_REMINDER)
            {
                foreach (var lightId in _deviceInfo.Lights)
                {
                    var ligth = GrainFactory.GetGrain<ILightGrain>(lightId);
                    await TurnLightOff(ligth);
                }
            }
        }
    }

    
}
