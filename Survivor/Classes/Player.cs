namespace Survivor.Classes
{
    public class Player
    {
        private int _health;
        private int _score;
        private int _maxHealth = 100;

        public int Health
        {
            get
            {
                return _health;
            }
        }
        public int Score
        {
            get
            {
                return _score;
            }
        }

        public Player()
        {
            _health = _maxHealth;
            _score = 0;
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