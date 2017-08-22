namespace BlepClick.Events
{
    public class DataCreditEvent : AciEvent
    {
        public byte DataCreditsAvailable => Content[1];

        public DataCreditEvent(byte[] content)
            : base(content)
        {
        }
    }
}
