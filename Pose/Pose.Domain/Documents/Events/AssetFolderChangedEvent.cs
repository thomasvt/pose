namespace Pose.Domain.Documents.Events
{
    internal class AssetFolderChangedEvent
    : IEvent
    {
        public string UndoPath { get; }
        public string NewPath { get; }

        public AssetFolderChangedEvent(string undoPath, string newPath)
        {
            UndoPath = undoPath;
            NewPath = newPath;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.SetAssetFolder(NewPath);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.SetAssetFolder(UndoPath);
        }
    }
}
