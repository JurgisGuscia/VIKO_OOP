namespace Survivor.Classes
{
    public class Position
    {
        private const int _startingX = 150;
        private const int _startingY = 150;

        private int _x;
        private int _y;
        
        private readonly WorldBounds _bounds;

        public (int X, int Y) Coords
        {
            get
            {
                return (_x, _y);
            }
        }

        public Position(WorldBounds bounds, int x  = _startingX, int y = _startingY)
        {
            _bounds = bounds;
            _x = x;
            _y = y;
        }



        public void Move(int x, int y)
        {

            _x += x;
            if (_x < _bounds.XStart)
                _x = _bounds.XStart;
            if (_x > _bounds.XEnd)
                _x = _bounds.XEnd;

            _y += y;
            if (_y < _bounds.YStart)
                _y = _bounds.YStart;
            if (_y > _bounds.YEnd)
                _y = _bounds.YEnd;
        }

        public void Reset()
        {
            _x = _startingX;
            _y = _startingY;
        }
    }
}