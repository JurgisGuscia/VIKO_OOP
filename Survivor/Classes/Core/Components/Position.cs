namespace Survivor.Classes.Core.Components
{
    public class ObjectPosition(float x, float y)
    {
        private Vector2 _startingPosition = new (x,y);
        private Vector2 _position = new(x,y);

        public Vector2 Position => _position;

        public void SetPosition(Vector2 coords) => _position = coords;
        public void Move(Vector2 movement) => _position += movement;
        public void Reset() => _position = _startingPosition;
    }
}