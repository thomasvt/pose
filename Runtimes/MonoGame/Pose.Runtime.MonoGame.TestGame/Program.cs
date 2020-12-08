using System;

namespace Pose.Runtime.MonoGame.TestGame
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new Game1();
            game.Run();
        }
    }
}
