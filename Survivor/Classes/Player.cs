using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoordVector = System.Numerics.Vector2;
using Vector2 = System.Numerics.Vector2;
namespace Survivor.Classes
{
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Attacking
    }

    public class Player : GameObject
    {
        private int _health;
        private int _score;
        private int _maxHealth = 100;

        private Texture2D spriteSheetIdle;
        private Texture2D spriteSheetRun;
        private Texture2D spriteSheetAttack;
        private Texture2D currentSprite;

        public string Direction { get; set; } = "right";
        public Texture2D SpriteSheet { get; set; }
        public int TotalFrames { get; set; } = 10;
        public PlayerState State { get; set; } = PlayerState.Jumping;

        public int Health => _health;
        public int Score => _score;

        public Player(WorldBounds bounds, Texture2D idleSheet, Texture2D runSheet, Texture2D attackSheet, Texture2D spriteSheetDead, int x = 150, int y = 150, int width = 120, int height = 200, float speedX = 0, float speedY = 0)
            : base(bounds, x, y, width, height, speedX, speedY)
        {
            spriteSheetIdle = idleSheet;
            spriteSheetRun = runSheet;
            spriteSheetAttack = attackSheet;
            currentSprite = spriteSheetIdle;
            _health = _maxHealth;
            _score = 0;
        }

        public void AddScore(int amount) => _score += amount;

        public void TakeDamage(int amount) => _health -= amount;

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
                State = PlayerState.Idle;
                YCoord = (int)Math.Round(WorldBounds.WorldEndingBounds.Y - 50);
            }
            CoordVector Coordinates = new(XCoord, YCoord);
            Position.SetCoords(Coordinates);
        }

        public void SelectSprite()
        {
            switch (State)
            {
                case PlayerState.Idle:
                    currentSprite = spriteSheetIdle;
                    TotalFrames = 10;
                    break;
                case PlayerState.Running:
                    currentSprite = spriteSheetRun;
                    TotalFrames = 16;
                    break;
                case PlayerState.Attacking:
                    currentSprite = spriteSheetAttack;
                    TotalFrames = 7;
                    break;
                default:
                    currentSprite = spriteSheetIdle;
                    TotalFrames = 10;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SelectSprite();

            SpriteEffects effects = Direction == "right" ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int frameWidth = currentSprite.Width / TotalFrames;
            int frameHeight = currentSprite.Height;
            int currentFrame = (int)(gameTime.TotalGameTime.TotalSeconds * 24) % TotalFrames;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            spriteBatch.Draw(
                 currentSprite,
                 new Rectangle((int)Math.Round(Position.Coords.X) - 92, (int)Math.Round(Position.Coords.Y) - 135, 180, 200),
                 sourceRect,
                 Color.White,
                 0f,
                 Vector2.Zero,
                 effects,
                 0f
                );
        }

        public override void Update(CoordVector playerPosition)
        {
            Velocity.ApplyVelocity();
            Position.Move(Velocity.Speed);
            Velocity.ResetVelocity();
            if (Velocity.Speed.Y == 0 && Velocity.Velocity.Y == 0 && Position.Coords.Y == WorldBounds.WorldEndingBounds.Y - 50)
            {
                State = PlayerState.Idle;
            }
            else
            {
                State = PlayerState.Jumping; 
            }
        }
    }
}