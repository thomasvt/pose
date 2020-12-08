namespace Pose.Domain.Editor.Messages
{
    public class EditorToolChanged
    {
        public EditorTool Tool { get; }

        public EditorToolChanged(EditorTool tool)
        {
            Tool = tool;
        }
    }
}
