using System.Collections.Generic;
using System.Linq;
using Pose.Persistence;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    public class SkeletonDefinitionFactory
    {
        private readonly ISpriteStore _spriteProvider;

        public SkeletonDefinitionFactory(ISpriteStore spriteProvider)
        {
            _spriteProvider = spriteProvider;
        }

        public SkeletonDefinition Create(Document document)
        {
            var uniqueSpriteFiles = document.Nodes.Where(n => n.Type == Persistence.Node.Types.NodeType.Spritenode)
                .Select(n => n.SpriteFile)
                .Distinct();

            var spriteStore = new Dictionary<string, SpriteQuad>();
            foreach (var spriteFilePath in uniqueSpriteFiles)
            {
                var sprite = _spriteProvider.GetSpriteQuad(spriteFilePath);
                spriteStore.Add(spriteFilePath, sprite);
            }

            return new SkeletonDefinition(document, spriteStore);
        }

        
    }
}
