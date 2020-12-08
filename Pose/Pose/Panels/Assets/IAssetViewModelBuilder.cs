using Pose.Domain;

namespace Pose.Panels.Assets
{
    public interface IAssetViewModelBuilder
    {
        SpriteViewModel Build(SpriteReference sprite);
    }
}