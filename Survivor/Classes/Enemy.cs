using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CoordVector = System.Numerics.Vector2;
namespace Survivor.Classes
{
    public enum EnemyState
    {
        Idle,
        Running,
        Jumping,
        Attacking,
        Dead
    }

    public class Enemy : GameObject
    {
        private int _health;

        public string Direction { get; set; } = "right";
        public int TotalFrames { get; set; } = 10;

        public Texture2D SpriteSheet { get; set; }
        public EnemyState State { get; set; } = EnemyState.Jumping;
        public int Health => _health;

        private Texture2D spriteSheetIdle;
        private Texture2D spriteSheetRun;
        private Texture2D spriteSheetAttack;
        private Texture2D currentSprite;
        private Texture2D spriteSheetDead;

        public int DeathFrames = 6;

        public Enemy(WorldBounds bounds, Texture2D idleSheet, Texture2D runSheet, Texture2D attackSheet, Texture2D deathSheet, int x = 150, int y = 120, int width = 120, int height = 200, float speedX = 0, float speedY = 0)
            : base(bounds, x, y, width, height, speedX, speedY)
        {
            spriteSheetIdle = idleSheet;
            spriteSheetRun = runSheet;
            spriteSheetAttack = attackSheet;
            spriteSheetDead = deathSheet;
            currentSprite = idleSheet;
            _health = 100;
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
                if (State != EnemyState.Dead)
                {
                    State = EnemyState.Idle;
                    YCoord = (int)Math.Round(WorldBounds.WorldEndingBounds.Y - 50);
                }
            }

            CoordVector Coordinates = new(XCoord, YCoord);
            Position.SetCoords(Coordinates);
        }



        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State != EnemyState.Dead)
            {
                switch (State)
                {
                    case EnemyState.Idle:
                        currentSprite = spriteSheetIdle;
                        TotalFrames = 8;
                        break;
                    case EnemyState.Running:
                        currentSprite = spriteSheetRun;
                        TotalFrames = 8;
                        break;
                    case EnemyState.Attacking:
                        currentSprite = spriteSheetAttack;
                        TotalFrames = 5;
                        break;
                    case EnemyState.Dead:
                        currentSprite = spriteSheetDead;
                        TotalFrames = 5;
                        break;
                    default:
                        currentSprite = spriteSheetIdle;
                        TotalFrames = 8;
                        break;
                }
            }
            else
            {
                currentSprite = spriteSheetDead;
                TotalFrames = 5;
            }

            SpriteEffects effects = Direction == "right" ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int frameWidth = currentSprite.Width / TotalFrames;
            int frameHeight = currentSprite.Height;
            int currentFrame = (int)(gameTime.TotalGameTime.TotalSeconds * 24) % TotalFrames;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            spriteBatch.Draw(
                 currentSprite,
                 new Rectangle((int)Math.Round(Position.Coords.X) - 50, (int)Math.Round(Position.Coords.Y) - 65, 100, 100),
                 sourceRect,
                 Color.White,
                 0f,
                 Vector2.Zero,
                 effects,
                 0f
                ); 
        }

        public CoordVector GetVelocity(CoordVector playerPosition)
        {
            
            if (playerPosition.X < Position.Coords.X)
            {
                Direction = "left";
                return (new(-0.1f, 0));
            }
            else
            {
                Direction = "right";
                return (new(0.1f, 0));
            }
            
        }

        public override void Update(CoordVector playerPosition)
        {
            if (State != EnemyState.Dead)
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
                {
                    State = EnemyState.Running;
                }
                else
                {
                    State = EnemyState.Jumping;
                }
            }
        }
    }
}