namespace BlepClick.Events
{
    public enum AciEventType : byte
    {
        DeviceStarted = 0x81,
        Echo = 0x82,
        CommandResponse = 0x84,
        Connected = 0x85,
        Disconnected = 0x86,
        BondStatus = 0x87,
        PipeStatus = 0x88,
        TimingEvent = 0x89,
        DataCredit = 0x8A,
        DataReceived = 0x8C,
        PipeError = 0x8D,
        DisplayKey = 0x8E
    }

    public static class AciEventTypeExtensions
    {

        public static string GetName(this AciEventType eventType)
        {
            switch (eventType)
            {
                case AciEventType.BondStatus:
                    return "BondStatus";

                case AciEventType.CommandResponse:
                    return "CommandResponse";

                case AciEventType.Connected:
                    return "Connected";

                case AciEventType.DataCredit:
                    return "DataCredit";

                case AciEventType.DataReceived:
                    return "DataReceived";

                case AciEventType.DeviceStarted:
                    return "DeviceStarted";

                case AciEventType.Disconnected:
                    return "Disconnected";

                case AciEventType.Echo:
                    return "Echo";

                case AciEventType.PipeError:
                    return "PipeError";

                case AciEventType.PipeStatus:
                    return "PipeStatus";

                case AciEventType.TimingEvent:
                    return "TimingEvent";

                default:
                    return eventType.ToString();
            }
        }
    }
}
