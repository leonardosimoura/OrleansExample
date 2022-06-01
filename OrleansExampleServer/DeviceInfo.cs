using Orleans;

namespace OrleansExampleGrainsInterfaces
{
    [Immutable]
    [GenerateSerializer]
    public record class DeviceInfo
    {
        public DeviceInfo(string name)
        {
            Name = name;
            Lights = new List<string>();
        }
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public List<string> Lights { get; set; }
        [Id(2)]
        public string Id { get; private set; }

        public void SetId(string id) => Id = id;
    }
    [Immutable]
    [GenerateSerializer]
    public record DeviceInfoDetail
    {
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public List<LightInfo> Lights { get; set; } = new List<LightInfo>();
        [Id(2)]
        public string Id { get; set; }
    }

}
