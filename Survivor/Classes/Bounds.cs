namespace Survivor.Classes
{
    public class Bounds
    {
        private int _xStart;
        private int _xEnd;
        private int _yStart;
        private int _yEnd;

        public Bounds()
        {
            _xStart = 0;
            _xEnd = 1280;
            _yStart = 0;
            _yEnd = 720;
        }

        public (int XS, int XE) XBounds
        {
            get
            {
                return (_xStart, _xEnd);
            }
        }

        public (int YS, int YE) YBounds
        {
            get
            {
                return (_yStart, _yEnd);
            }
        }
    }
}