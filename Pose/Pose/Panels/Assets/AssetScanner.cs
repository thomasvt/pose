using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pose.Domain;

namespace Pose.Panels.Assets
{
    public class AssetScanner : IAssetScanner
    {
        public async Task<List<SpriteReference>> Scan(string folder)
        {
            return await Task.Run(() =>
            {
                var files = Directory.GetFiles(folder, "*.png");
                
                return files.Select(path =>
                {
                    var relativePath = Path.GetRelativePath(folder, path);
                    return new SpriteReference(relativePath);
                }).ToList();
            });
        }
    }
}
