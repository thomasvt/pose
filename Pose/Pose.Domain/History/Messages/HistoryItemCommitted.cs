namespace Pose.Domain.History.Messages
{
    public class HistoryItemCommitted
    {
        public ulong Version { get; }
        public string Label { get; }

        public HistoryItemCommitted(ulong version, string label)
        {
            Version = version;
            Label = label;
        }
    }
}
