using System.Numerics;
using System;

namespace Survivor.Classes
{
    public class Size(int x, int y)
    {
        private Vector2 _objectSize = new(x, y);

        public Vector2 ObjectSize => _objectSize;

        public Vector2 StartPoint(Vector2 position)
        {
            float startX = (int)Math.Round(position.X) - _objectSize.X / 2;
            float startY = (int)Math.Round(position.Y) - _objectSize.Y / 2;
            Vector2 StartPoint = new(startX, startY);
            return StartPoint;
        }

        public Vector2 EndPoint(Vector2 position)
        {
            float endX = (int)Math.Round(position.X) + _objectSize.X / 2;
            float endY = (int)Math.Round(position.Y) + _objectSize.Y / 2;
            Vector2 EndPoint = new(endX, endY);
            return EndPoint;
        }
    }
}