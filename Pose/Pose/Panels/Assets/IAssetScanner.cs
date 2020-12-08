using System.Collections.Generic;
using System.Threading.Tasks;
using Pose.Domain;

namespace Pose.Panels.Assets
{
    public interface IAssetScanner
    {
        Task<List<SpriteReference>> Scan(string folder);
    }
}