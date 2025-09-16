using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Core.Interfaces;
namespace Survivor.Classes.Core
{
    public class Player : GameObject
    {
        private int _health;
        private int _mana;
        private int _score;
        private int _maxHealth = 100;
        private int _maxMana = 100;
        private readonly IWorldBounds _worldBounds;
        public int Health => _health;
        public int Score => _score;
        public int _manaRegenerationTicks = 200;
        public int _manaRegenerationCoolDown = 0;

        private readonly Animator _Animator;

        public string Direction { get; set; } = "right";
        public State State { get; set; } = State.Idle;

        public Player(IWorldBounds worldBounds, Animator.DrawData drawData, Vector2 playerPosition, Vector2 boxSize)
            : base((int)playerPosition.X, (int)playerPosition.Y, (int)boxSize.X, (int)boxSize.Y)
        {
            _health = _maxHealth;
            _mana = _maxMana;
            _score = 0;
            _worldBounds = worldBounds;
            _Animator = new Animator(drawData, State.Idle);
        }

        public void AddScore(int amount) => _score += amount;
        public void AddHealth(int amount)
        {
            _health += amount;
            if (_health > 100)
                _health = 100;
        }
        public void RegenerateMana(int mana)
        {
            if (_manaRegenerationCoolDown <= 0)
            {
                _mana += mana;
                if (_mana > 100)
                    _mana = 100;
                _manaRegenerationCoolDown = _manaRegenerationTicks;
            }
            else
                _manaRegenerationCoolDown--;
            
        } 
        public void ReduceMana(int mana) => _mana -= mana;
        public int Mana => _mana;
        
        public void TakeDamage(int amount) => _health -= amount;

        public void SetState(State state)
        {
            State = state;
            _Animator.SetState(state);
        }

       public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
            _Animator.Draw(spriteBatch, Position.Position, Direction, gameTime);

        public override void Update(Vector2 playerPosition)
        {
            RegenerateMana(5);
            Velocity.AddVelocity();
            Position.Move(Velocity.Velocity);
            Velocity.ResetAcceleration();
            if (Position.Position.Y >= _worldBounds.WorldEnd.Y - Size.Size.Y / 2 - 20)
            {
                SetState(State.Idle);
                Velocity.ResetVelocityY();
            }
            else
                SetState(State.Jumping);
        }
    }
}