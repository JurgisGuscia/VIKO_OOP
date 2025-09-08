using Microsoft.Xna.Framework.Graphics;

namespace Survivor.Classes
{
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Attacking
    }

    public class Player
    {
        private int _health;
        private int _score;
        private int _maxHealth = 100;
        private Position Position;

        private readonly WorldBounds _bounds;

        public PlayerState State { get; set; } = PlayerState.Idle;
        public string direction;

        public int Health => _health;
        public int Score => _score;

        public (int X, int Y) Coordinates
        {
            get
            {
                return (Position.Coords.X, Position.Coords.Y);
            }

        }

        public void Move(int x, int y)
        {
            Position.Move(x, y);
        }

        public Player(int x, int y)
        {
            _health = _maxHealth;
            _score = 0;
            _bounds = new WorldBounds();
            Position = new Position(_bounds, x, y);
            direction = "right";
        }

        public void AddScore(int amount)
        {
            _score += amount;
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health < 0)
                _health = 0;
        }

        public void Heal(int amount)
        {
            _health += amount;
            if (_health > _maxHealth)
                _health = _maxHealth;
        }

        public void Reset()
        {
            _health = _maxHealth;
            _score = 0;
        }
    }
}