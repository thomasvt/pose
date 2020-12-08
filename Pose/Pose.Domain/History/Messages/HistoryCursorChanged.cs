namespace Pose.Domain.History.Messages
{
    public class HistoryCursorChanged
    {
        public ulong Version { get; }

        public HistoryCursorChanged(ulong version)
        {
            Version = version;
        }
    }
}
