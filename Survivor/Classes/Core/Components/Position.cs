using System.Numerics;

namespace Survivor.Classes.Core.Components
{
    public class Position(float x, float y)
    {
        private Vector2 _startingPosition = new (x,y);
        private Vector2 _position = new(x,y);

        public Vector2 Coords => _position;

        public void SetCoords(Vector2 coords) => _position = coords;

        public void Move(Vector2 movement) => _position += movement;

        public void Reset() => _position = _startingPosition;
    }
}