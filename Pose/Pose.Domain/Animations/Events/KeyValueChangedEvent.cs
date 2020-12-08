using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class KeyValueChangedEvent
    : IEvent
    {
        public ulong KeyId { get; }
        public float UndoValue { get; }
        public float Value { get; }

        public KeyValueChangedEvent(ulong keyId, float undoValue, float value)
        {
            KeyId = keyId;
            UndoValue = undoValue;
            Value = value;
        }

        public void PlayForward(IEditableDocument document)
        {
            var key = document.GetKey(KeyId) as IEditableKey;
            key.ChangeValue(Value);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var key = document.GetKey(KeyId) as IEditableKey;
            key.ChangeValue(UndoValue);
        }
    }
}
