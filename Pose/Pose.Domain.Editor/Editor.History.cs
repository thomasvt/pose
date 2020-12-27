namespace Pose.Domain.Editor
{
    /// <summary>
    /// Actions directly involving undo History.
    /// </summary>
    public partial class Editor
    {
        public bool CanUndo()
        {
            return CurrentDocument.CanUndo();
        }

        public void Undo()
        {
            CurrentDocument.Undo();
        }

        public bool CanRedo()
        {
            return CurrentDocument.CanRedo();
        }

        public void Redo()
        {
            CurrentDocument.Redo();
        }

        /// <summary>
        /// Plays history backward or forward until a certain document version is reached.
        /// </summary>
        public void NavigateHistoryTo(in ulong version)
        {
            CurrentDocument.NavigateHistoryTo(version);
        }
    }
}
