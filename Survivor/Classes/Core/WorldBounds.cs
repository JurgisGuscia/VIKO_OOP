using Survivor.Classes.Core.Interfaces;
namespace Survivor.Classes.Core
{
    public class WorldBounds : IWorldBounds
    {
        private Vector2 _worldSize;
        private Vector2 _worldStart;
        private Vector2 _worldEnd;

        public WorldBounds() : this(new Vector2(1280, 720))
        {}

        public WorldBounds(Vector2 worldSize)
        {
            _worldSize = worldSize;
            _worldStart = Vector2.Zero;
            _worldEnd = _worldSize;
        }
        
        public Vector2 WorldSize => _worldSize;
        public Vector2 WorldStart => _worldStart;
        public Vector2 WorldEnd => _worldEnd;

        public void SetWorldSize(Vector2 newSize)
        {
            _worldSize = newSize;
            _worldEnd = newSize;
        }
    }
}