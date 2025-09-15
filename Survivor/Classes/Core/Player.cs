using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Core.Interfaces;

namespace Survivor.Classes.Core
{
    public class Player : GameObject
    {
        private int _health;
        private int _score;
        private int _maxHealth = 100;
        private readonly IWorldBounds _worldBounds;
        public int Health => _health;
        public int Score => _score;

        private readonly Animator _Animator;

        public string Direction { get; set; } = "right";
        public State State { get; set; } = State.Idle;

        public Player(IWorldBounds worldBounds, Animator.DrawData drawData, Vector2 playerPosition, Vector2 boxSize)
            : base((int)playerPosition.X, (int)playerPosition.Y, (int)boxSize.X, (int)boxSize.Y)
        {
            _health = _maxHealth;
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