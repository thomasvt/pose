namespace Pose.Domain.Editor.Messages
{
    public class KeySelected
    {
        public ulong KeyId { get; }

        public KeySelected(ulong keyId)
        {
            KeyId = keyId;
        }
    }
}
