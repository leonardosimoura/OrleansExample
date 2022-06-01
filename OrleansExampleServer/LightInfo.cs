using Orleans;

namespace OrleansExampleGrainsInterfaces
{
    [Immutable]
    [GenerateSerializer]
    public record class LightInfo
    {
        public LightInfo(string name, bool on)
        {
            Name = name;
            On = on;
        }

        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public bool On { get; set; }
        [Id(2)]
        public string Id { get; private set; }

        public void SetId(string id) => Id = id;
    }

}
