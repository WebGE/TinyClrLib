namespace BlepClick.Events
{
    public class DeviceStartedEvent : AciEvent
    {
        public Nrf8001State State => (Nrf8001State)Content[1];

        public byte DataCreditsAvailable => Content[3];

        public DeviceStartedEvent(byte[] content)
            : base(content)
        {
        }
    }
}
