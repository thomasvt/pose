namespace Pose.Domain.Documents.Messages
{
    public class MetaDataChanged
    {
        public string Key { get; }
        public object Value { get; }

        public MetaDataChanged(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
