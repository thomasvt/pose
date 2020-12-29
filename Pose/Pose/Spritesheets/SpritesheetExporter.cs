using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Pose.Persistence.Editor;
using Pose.SpritePacker;
using SkiaSharp;

namespace Pose.Spritesheets
{
    public static class SpritesheetExporter
    {
        /// <summary>
        /// Creates and saves a single png spritesheet from a collection of sprites.
        /// </summary>
        public static void Export(IEnumerable<SpriteInfo> sprites, string filenamePng, string filenameJson)
        {
            // calculate the spritesheet layout
            var spritesheet = new SpritePacker.SpritePacker().Pack(sprites.Select(sprite => new Sprite(sprite, sprite.Bitmap.Width, sprite.Bitmap.Height)));

            // create the spritsheet bitmap
            using var bitmap = new SKBitmap(new SKImageInfo(spritesheet.Width, spritesheet.Height));
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColor.Empty);
                foreach (var sprite in spritesheet.Sprites)
                {
                    var spriteInfo = (SpriteInfo) sprite.Sprite.Reference;
                    canvas.DrawBitmap(spriteInfo.Bitmap, sprite.X, sprite.Y);
                }
            }

            SaveImage(filenamePng, bitmap);
            SaveData(filenameJson, spritesheet);
        }

        private static void SaveImage(string filename, SKBitmap bitmap)
        {
            using var stream = File.OpenWrite(filename);
            bitmap.Encode(stream, SKEncodedImageFormat.Png, 0);
        }

        private static void SaveData(string filename, Spritesheet spritesheet)
        {
            var protoSpritesheet = new Persistence.Spritesheet
            {
                Width = (uint) spritesheet.Width,
                Height = (uint) spritesheet.Height
            };
            foreach (var sprite in spritesheet.Sprites)
            {
                var spriteInfo = (SpriteInfo)sprite.Sprite.Reference;
                protoSpritesheet.Sprites.Add(new Persistence.Sprite
                {
                    Key = spriteInfo.Key,
                    X = (uint)sprite.X,
                    Y = (uint)sprite.Y,
                    Width = (uint)sprite.Sprite.Width,
                    Height = (uint)sprite.Sprite.Height
                });
            }

            ProtobufSaver.Save(protoSpritesheet, filename);
        }
    }
}
