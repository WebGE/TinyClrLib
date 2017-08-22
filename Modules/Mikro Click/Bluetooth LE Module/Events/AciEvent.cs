namespace BlepClick.Events
{
    public class AciEvent
    {
        public AciEventType EventType { get; }
        public byte[] Content { get; }

        public AciEvent(byte[] content)
        {
            EventType = (AciEventType) content[0];
            Content = content;
        }
    }
}
