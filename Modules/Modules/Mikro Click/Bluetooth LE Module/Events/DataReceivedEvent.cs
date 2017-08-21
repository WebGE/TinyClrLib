using System;

namespace BlepClick.Events
{
    public class DataReceivedEvent : AciEvent
    {
        public byte ServicePipeId => Content[1];

        public byte[] Data { get; }
        public DataReceivedEvent(byte[] content) : base(content)
        {
            Data = new byte[content.Length - 2];
            Array.Copy(content,2,Data,0,Data.Length);
        }
    }
}
