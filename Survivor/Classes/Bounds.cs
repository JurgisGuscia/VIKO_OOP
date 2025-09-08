namespace Survivor.Classes
{
    public class WorldBounds
    {
        private readonly int _xStart;
        private readonly int _xEnd;
        private readonly int _yStart;
        private readonly int _yEnd;

        public WorldBounds(int width = 1280, int height = 720)
        {
            _xStart = 0;
            _yStart = 0;
            _xEnd = width;
            _yEnd = height;
        }

        public int XStart => _xStart;
        public int XEnd => _xEnd;
        public int YStart => _yStart;
        public int YEnd => _yEnd;

        public int Width => _xEnd - _xStart;
        public int Height => _yEnd - _yStart;
    }
}