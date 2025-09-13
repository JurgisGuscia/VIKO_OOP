using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoordVector = System.Numerics.Vector2;
using Vector2 = System.Numerics.Vector2;
using Survivor.Classes.Core.Components;
namespace Survivor.Classes.Core
{
    public class Player : GameObject
    {
        private int _health;
        private int _score;
        private int _maxHealth = 100;
        
        public int Health => _health;
        public int Score => _score;

        private readonly Animator _Animator;

        public string Direction { get; set; } = "right";
        public State State { get; set; } = State.Idle;

        public Player(WorldBounds bounds, Animator.DrawData drawData, Vector2 playerPosition, Vector2 boxSize)
            : base(bounds, (int)playerPosition.X, (int)playerPosition.Y, (int)boxSize.X, (int)boxSize.Y)
        {
            _health = _maxHealth;
            _score = 0;
            _Animator = new Animator(drawData, State.Idle);
        }

        public void AddScore(int amount) => _score += amount;

        public void TakeDamage(int amount) => _health -= amount;

        public void SetState(State state)
        {
            State = state;
            _Animator.SetState(state);
        }

        public void HandleOutOfBounds()
        {
            int XCoord = (int)Math.Round(Position.Coords.X);
            int YCoord = (int)Math.Round(Position.Coords.Y);

            if (XCoord < WorldBounds.WorldStartingBounds.X)
                XCoord = (int)Math.Round(WorldBounds.WorldStartingBounds.X);

            if (YCoord < WorldBounds.WorldStartingBounds.Y)
                YCoord = (int)Math.Round(WorldBounds.WorldStartingBounds.Y);

            if (XCoord > WorldBounds.WorldEndingBounds.X)
                XCoord = (int)Math.Round(WorldBounds.WorldEndingBounds.X);

            if (YCoord > WorldBounds.WorldEndingBounds.Y - 50)
            {
                SetState(State.Idle);
                YCoord = (int)Math.Round(WorldBounds.WorldEndingBounds.Y - 50);
            }
            CoordVector Coordinates = new(XCoord, YCoord);
            Position.SetCoords(Coordinates);
        }

       public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
            _Animator.Draw(spriteBatch, Position.Coords, Direction, gameTime);

        public override void Update(CoordVector playerPosition)
        {
            Velocity.ApplyVelocity();
            Position.Move(Velocity.Speed);
            Velocity.ResetVelocity();
            if (Velocity.Speed.Y == 0 && Velocity.Velocity.Y == 0 && Position.Coords.Y == WorldBounds.WorldEndingBounds.Y - 50)
                SetState(State.Idle);
            else
                SetState(State.Jumping);
        }
    }
}