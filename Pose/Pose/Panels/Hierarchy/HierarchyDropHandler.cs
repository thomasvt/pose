using System.Linq;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Pose.Domain.Documents;
using Pose.Domain.Editor;

namespace Pose.Panels.Hierarchy
{
    public class HierarchyDropHandler : DefaultDropHandler
    {
        private readonly Editor _editor;

        public HierarchyDropHandler(Editor editor)
        {
            _editor = editor;
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
                return;

            var nodeId = (dropInfo.Data as HierarchyNodeViewModel).NodeId;
            var targetViewModel = dropInfo.TargetItem as HierarchyNodeViewModel;
            
            var insertionPlace = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
                ? InsertPosition.Child
                : dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem)
                    ? InsertPosition.After
                    : InsertPosition.Before;

            if (targetViewModel == null)
            {
                // dropped onto nothing, convert to its equivalent: drop after the last rootnode
                targetViewModel = dropInfo.TargetCollection.TryGetList().OfType<HierarchyNodeViewModel>().Last();
                insertionPlace = InsertPosition.After;
            }
            
            _editor.MoveNodeInHierarchy(nodeId, targetViewModel.NodeId, insertionPlace);
        }
    }
}
