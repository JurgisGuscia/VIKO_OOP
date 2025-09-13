using System.Numerics;
using System;
using Survivor.Classes.Core.Components;
namespace Survivor.Classes.Core
{
    public enum State
    {
        Idle,
        Running,
        Jumping,
        Attacking,
        Dead
    }
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
            bool withinX = startPoint.X >= WorldStartingBounds.X && endPoint.X <= WorldEndingBounds.X;
            bool withinY = startPoint.Y >= WorldStartingBounds.Y && endPoint.Y <= WorldEndingBounds.Y;
            return withinX && withinY;
        }


    }
}