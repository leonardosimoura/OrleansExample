using Orleans;

namespace OrleansExampleGrainsInterfaces
{
    public interface ILightGrain : IGrainWithStringKey
    {

        Task<string> Name();

        Task SetInfo(LightInfo info);

        Task<LightInfo> Info();

        Task TurnLightOn();

        Task TurnLightOff();
    }

}
