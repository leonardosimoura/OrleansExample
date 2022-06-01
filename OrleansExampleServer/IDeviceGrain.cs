using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansExampleGrainsInterfaces
{
    public interface IDeviceGrain : IGrainWithStringKey
    {

        Task<string> Name();

        Task SetInfo(DeviceInfo info);

        Task<DeviceInfo> Info();

        Task<DeviceInfoDetail> InfoDetail();

        Task TurnLightOn(ILightGrain light);

        Task TurnLightOff(ILightGrain light);
    }

}
