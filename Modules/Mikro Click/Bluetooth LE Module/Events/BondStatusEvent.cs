using BlepClick.Commands;

namespace BlepClick.Events
{
    public class BondStatusEvent:AciEvent
    {
        public BondStatusCode StatusCode => (BondStatusCode) Content[1];

        public BondStatusEvent(byte[] content) : base(content)
        {
        }
    }
}
