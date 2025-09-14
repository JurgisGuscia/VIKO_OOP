using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Core.Interfaces;

namespace Survivor.Classes.Core
{
    public class Enemy : GameObject
    {
        private int _health;

        private readonly Animator _Animator;
        private readonly IWorldBounds _worldBounds;
        public string Direction { get; set; } = "right";
        public int TotalFrames { get; set; } = 10;
        public State State { get; set; } = State.Running;
        public int Health => _health;
        public Texture2D SpriteSheet { get; set; }

        public bool AnimationFinished() => _Animator.AnimationFinished();
        public void StartDeathAnimation() => _Animator.StartDeathAnimation();
        
       public Enemy(IWorldBounds worldBounds, Animator.DrawData drawData, Vector2 boxSize, Vector2 position)
            : base((int)position.X, (int)position.Y, (int)boxSize.X, (int)boxSize.Y)
        {
            _worldBounds = worldBounds;
             _Animator = new Animator(drawData, State.Idle);
            _health = 100;
        }
        
        public void SetState(State state)
        {
            State = state;
            _Animator.SetState(state);
        } 

        public Vector2 GetVelocity(Vector2 playerPosition)
        {
            if (playerPosition.X < Position.Position.X)
            {
                Direction = "left";
                return new(-0.1f, 0);
            }
            else
            {
                Direction = "right";
                return new(0.1f, 0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)=>
            _Animator.Draw(spriteBatch, Position.Position, Direction, gameTime);

        public override void Update(Vector2 playerPosition)
        {
            if (State != State.Dead)
            {
                Velocity.ApplyForce(GetVelocity(playerPosition));
                Velocity.AddVelocity();
                Position.Move(Velocity.Velocity);
                Velocity.ResetAcceleration();
                
                if (Velocity.Velocity.X > 2)
                    Velocity.SetVelocityX(2f);

                if (Velocity.Velocity.X < -2)
                    Velocity.SetVelocityX(-2f);

                if (Position.Position.Y >= _worldBounds.WorldEnd.Y - Size.Size.Y / 2)
                    SetState(State.Running);
                else
                    SetState(State.Idle);
             
            }
        }
    }
}