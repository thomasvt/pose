namespace Pose.Domain.History.Messages
{
    /// <summary>
    /// Triggered when the history after a certain version (inclusive) is removed.
    /// </summary>
    public class HistoryRemovedAfter
    {
        public ulong Version { get; }

        public HistoryRemovedAfter(ulong version)
        {
            Version = version;
        }
    }
}
