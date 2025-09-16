using System;
using Survivor.Classes.Core.Interfaces;
namespace Survivor.Classes.Core.Utils
{
    public static class Util
    {
        public static int ToInt(float number) => (int)Math.Round(number);

        public static Vector2 PickSpawnLocation(IWorldBounds bounds)
        {
            Random random = new Random();
            float x = 0;
            float y = 0;
            int pickSide = random.Next(1, 3);
            if (pickSide == 1)
                x = random.Next(ToInt(bounds.WorldEnd.X * 0.05f), ToInt(bounds.WorldEnd.X * 0.2f));
            else
                x = random.Next(ToInt(bounds.WorldEnd.X * 0.8f), ToInt(bounds.WorldEnd.X * 0.95f));
            y = random.Next(40, ToInt(bounds.WorldEnd.Y) - 200);
            return new(x, y);
        }
    }

}