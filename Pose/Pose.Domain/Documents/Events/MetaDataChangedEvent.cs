namespace Pose.Domain.Documents.Events
{
    internal class MetaDataChangedEvent
    : IEvent
    {
        public string Key { get; }
        public object UndoValue { get; }
        public object Value { get; }

        public MetaDataChangedEvent(string key, object undoValue, object value)
        {
            Key = key;
            UndoValue = undoValue;
            Value = value;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.SetMetaDataValue(Key, Value);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.SetMetaDataValue(Key, UndoValue);
        }
    }
}
