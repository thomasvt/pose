namespace Pose.Domain.History
{
    internal interface IHistory
    {
        void Undo();
        void Redo();
        bool CanRedo();
        bool CanUndo();
        IUnitOfWork StartUnitOfWork(string name);
    }
}