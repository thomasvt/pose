using System.Collections.Generic;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Pose.Domain.Editor;

namespace Pose.Panels.DrawOrder
{
    public class DrawOrderDropHandler : DefaultDropHandler
    {
        private readonly Editor _editor;

        public DrawOrderDropHandler(Editor editor)
        {
            _editor = editor;
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
                return;
            
            var targetList = dropInfo.TargetCollection.TryGetList();
            
            var sourceIndex = targetList.IndexOf(dropInfo.Data);
            var destinationIndex = dropInfo.InsertIndex;

            if (sourceIndex == destinationIndex)
                return;

            var subjectViewModel = (DrawOrderItemViewModel) dropInfo.Data;
            _editor.MoveSpriteNodeInFrontOfTarget(subjectViewModel.NodeId, destinationIndex);
        }
    }
}
