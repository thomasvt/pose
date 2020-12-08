namespace Pose.Domain.History
{
    public interface IHistory
    {
        void Undo();
        void Redo();
        bool CanRedo();
        bool CanUndo();
        IUnitOfWork StartUnitOfWork(string name);
    }
}