using System.Numerics;
using System;

namespace Survivor.Classes.Core.Components
{
    public class Size(int x, int y)
    {
        private Vector2 _objectSize = new(x, y);

        public Vector2 ObjectSize => _objectSize;
        
        public Vector2 StartPoint(Vector2 position) =>
            new((int)Math.Round(position.X) - _objectSize.X / 2, (int)Math.Round(position.Y) - _objectSize.Y / 2);

        public Vector2 EndPoint(Vector2 position) =>
            new((int)Math.Round(position.X) + _objectSize.X / 2, (int)Math.Round(position.Y) + _objectSize.Y / 2);
    }
}