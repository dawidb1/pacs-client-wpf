namespace pacs_client.Model
{
    public class PacsConfiguration : IPacsConfiguration
    {
        public string myAET { get; } = "KLIENTL";

        public string callAET { get; } = "ARCHIWUM";

        public string ipPACS { get; } = "127.0.0.1";

        public ushort portPACS { get; } = 10100;

        public ushort portMove { get; } = 10104;
    }
}