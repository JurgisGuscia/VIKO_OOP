namespace Survivor.Classes
{
    public class Position
    {
        private int _startingX = 150;
        private int _startingY = 150;

        private int _x;
        private int _y;

        public (int X, int Y) Coords
        {
            get
            {
                return(_x,_y);
            }
        }

        public Position()
        {
            _x = _startingX;
            _y = _startingY;
        }
        
        

        public void Move(int x, int y)
        {
            _x += x;
            _y += y;
        }

        public void Reset()
        {
            _x = _startingX;
            _y = _startingY;
        }
    }
}