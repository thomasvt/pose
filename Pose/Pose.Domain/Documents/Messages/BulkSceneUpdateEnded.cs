namespace Pose.Domain.Documents.Messages
{
    /// <summary>
    /// Nodes in the scene can be involved in a bulk update: an update changing many node properties, causing exponential amounts of redundant logical updates throughout the application.
    /// During a bulk update, nodes will send a isBulkUpdate flag along with their individual change messages. Recipients can choose to ignore these individual messages when that isBulkUpdate flag is true.
    /// They can update their entire state in one go afterwards by listening for <see cref="BulkSceneUpdateEnded"/>.
    /// </summary>
    public class BulkSceneUpdateEnded
    {
    }
}
