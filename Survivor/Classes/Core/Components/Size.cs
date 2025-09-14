using Survivor.Classes.Core.Utils;
namespace Survivor.Classes.Core.Components
{
    public class ObjectSize
    {
        private Vector2 _size;

        public ObjectSize(int x, int y) => _size = new Vector2(x, y);

        public Vector2 Size => _size;

        public Vector2 StartPoint(Vector2 position) =>
            new(Util.ToInt(position.X) - _size.X / 2, Util.ToInt(position.Y) - _size.Y / 2);

        public Vector2 EndPoint(Vector2 position) =>
            new(Util.ToInt(position.X) + _size.X / 2, Util.ToInt(position.Y) + _size.Y / 2);
    }
}