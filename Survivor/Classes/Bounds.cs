using System.Numerics;
using System;
namespace Survivor.Classes
{
    public class WorldBounds(float width = 1280, float height = 720)
    {
        private readonly Vector2 _end = new(width, height);

        public static Vector2 WorldStartingBounds => Vector2.Zero;
        public Vector2 WorldEndingBounds => _end;

        public int WorldWidth => (int)Math.Round(_end.X);
        public int WorldHeight => (int)Math.Round(_end.Y);

        public bool Contains(Vector2 position, Size size)
        {
            Vector2 startPoint = size.StartPoint(position);
            Vector2 endPoint = size.EndPoint(position);
            if (
                startPoint.X >= WorldStartingBounds.X &&
                startPoint.X <= WorldEndingBounds.X &&
                endPoint.Y >= WorldStartingBounds.Y &&
                endPoint.Y <= WorldEndingBounds.Y
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
            

    }
}