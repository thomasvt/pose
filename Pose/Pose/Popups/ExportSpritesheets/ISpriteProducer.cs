using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Pose.SceneEditor.Viewport;

namespace Pose.Popups.ExportSpritesheets
{
    public interface ISpriteProducer
    {
        void PrepareDocument();
        RenderTargetBitmap ProduceFirstAnimationFrame(ulong animationId);
        List<RenderTargetBitmap> ProduceAnimationFrames(ulong animationId);
        float Scale { get; set; }
        double DpiX { get; set; }
        double DpiY { get; set; }
        SceneViewport SceneViewport { get; set; }
    }
}