namespace Pose.Domain.Editor.Messages
{
    public class KeyDeselected
    {
        public readonly ulong KeyId;

        public KeyDeselected(ulong keyId)
        {
            KeyId = keyId;
        }
    }
}
