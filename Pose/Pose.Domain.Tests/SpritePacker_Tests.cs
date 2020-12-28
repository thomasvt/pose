using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pose.SpritePacker.Tests
{
    [TestClass]
    public class SpritePacker_Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var packer = new SpritePacker();
            var result = packer.Pack(new[] {new Sprite("a", 10, 4), new Sprite("b", 4, 4), new Sprite("c", 4, 4), new Sprite("d", 3, 3)});

            Console.WriteLine($"Result = {result.Width}x{result.Height}");

            foreach (var sprite in result.Sprites)
            {
                Console.WriteLine($"{sprite.Sprite.Name} = {sprite.X}x{sprite.Y} {sprite.Sprite.Width}x{sprite.Sprite.Height}");
            }
        }
    }
}
