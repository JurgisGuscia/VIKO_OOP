using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CoordVector = System.Numerics.Vector2;
using Survivor.Classes.Core.Components;
namespace Survivor.Classes.Core
{
    public class Enemy : GameObject
    {
        private int _health;

        private readonly Animator _Animator;
        
        public string Direction { get; set; } = "right";
        public int TotalFrames { get; set; } = 10;
        public State State { get; set; } = State.Running;
        public int Health => _health;
        public Texture2D SpriteSheet { get; set; }

        public bool AnimationFinished() => _Animator.AnimationFinished();
        public void StartDeathAnimation() => _Animator.StartDeathAnimation();
        
       public Enemy(WorldBounds bounds, Animator.DrawData drawData, Vector2 boxSize, Vector2 position)
            : base(bounds, (int)position.X, (int)position.Y, (int)boxSize.X, (int)boxSize.Y)
        {
             _Animator = new Animator(drawData, State.Idle);
            _health = 100;
        }
        
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
                if (State != State.Dead)
                {
                    SetState(State.Idle);
                    YCoord = (int)Math.Round(WorldBounds.WorldEndingBounds.Y - 50);
                }
            }

            CoordVector Coordinates = new(XCoord, YCoord);
            Position.SetCoords(Coordinates);
        }

        public CoordVector GetVelocity(CoordVector playerPosition)
        {
            if (playerPosition.X < Position.Coords.X)
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
            _Animator.Draw(spriteBatch, Position.Coords, Direction, gameTime);

        public override void Update(CoordVector playerPosition)
        {
            if (State != State.Dead)
            {
                Velocity.AddVelocity(GetVelocity(playerPosition));
                Velocity.ApplyVelocity();
                Position.Move(Velocity.Speed);
                Velocity.ResetVelocity();

                if (Velocity.Speed.X > 2)
                    Velocity.SetSpeedX(2f);

                if (Velocity.Speed.X < -2)
                    Velocity.SetSpeedX(-2f);

                if (Velocity.Speed.Y == 0 && Velocity.Velocity.Y == 0 && Position.Coords.Y == WorldBounds.WorldEndingBounds.Y - 50)
                    SetState(State.Running);
                else
                    SetState(State.Jumping);
            }
        }
    }
}