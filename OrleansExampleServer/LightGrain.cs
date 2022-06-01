using Orleans;
using OrleansExampleGrainsInterfaces;

namespace OrleansExampleGrains
{
    public class LightGrain : Grain, ILightGrain
    {
        private LightInfo _lightInfo;

        public Task<LightInfo> Info()
        {
            return Task.FromResult(_lightInfo);
        }

        public Task<string> Name()
        {
            return Task.FromResult(_lightInfo!.Name);
        }

        public Task SetInfo(LightInfo info)
        {
            _lightInfo = info;
            _lightInfo.SetId(base.GrainReference.GetPrimaryKeyString());
            return Task.CompletedTask;
        }

        public Task TurnLightOff()
        {
            _lightInfo!.On = false;
            return Task.CompletedTask;
        }

        public Task TurnLightOn()
        {
            _lightInfo!.On = true;
            return Task.CompletedTask;
        }
    }
}
