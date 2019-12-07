namespace pacs_client.Model
{
    public interface IPacsConfiguration
    {
        string myAET { get; } // moj AET - ustaw zgodnie z konfiguracją serwera PACS
        string callAET { get; }    // AET serwera - j.w.
        string ipPACS { get; }    // IP serwera - j.w.
        ushort portPACS { get; }        // port serwera - j.w.
        ushort portMove { get; }        // port zwrotny dla MOVE - j.w.
    }
}