namespace Pose.Domain.Nodes
{
    internal interface IEditableNodeCollection
    {
        void Attach(int index, Node node);
        void Detach(Node node);
    }
}