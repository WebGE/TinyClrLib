using BlepClick.Commands;

namespace BlepClick.Events
{
    public class CommandResponseEvent : AciEvent
    {
        public AciOpCode Command => (AciOpCode)Content[1];
        public AciStatusCode StatusCode => (AciStatusCode) Content[2];

        public CommandResponseEvent(byte[] content) : base(content)
        {
        }
    }
}