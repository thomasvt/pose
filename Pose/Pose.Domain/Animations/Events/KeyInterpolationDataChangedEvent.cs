using System;
using Pose.Domain.Documents;

namespace Pose.Domain.Animations.Events
{
    internal class KeyInterpolationDataChangedEvent
    : IEvent
    {
        public readonly ulong KeyId;
        public readonly InterpolationData UndoData;
        public readonly InterpolationData Data;

        public KeyInterpolationDataChangedEvent(in ulong keyId, InterpolationData undoData, InterpolationData data)
        {
            KeyId = keyId;
            UndoData = undoData;
            Data = data;
        }

        public void PlayForward(IEditableDocument document)
        {
            var key = document.GetKey(KeyId) as IEditableKey;
            key.ChangeInterpolationData(Data);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var key = document.GetKey(KeyId) as IEditableKey;
            key.ChangeInterpolationData(UndoData);
        }
    }
}
