using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Survivor.Classes;

namespace Survivor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D spriteSheetIdle;
        Texture2D spriteSheetRun;
        Texture2D backgroundTexture;
        Texture2D currentSprite;
        Texture2D landTexture;

        private WorldBounds _worldBounds;
        private Player _player; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _worldBounds = new WorldBounds();
            
            _graphics.PreferredBackBufferWidth = _worldBounds.Width; 
            _graphics.PreferredBackBufferHeight = _worldBounds.Height;
            _graphics.ApplyChanges();

        }


        private void HandlePlayerInput()
        {
           _player.State = PlayerState.Idle;
            var keyboardState = Keyboard.GetState();
            int dx = 0;
            int dy = 0;

            // if (keyboardState.IsKeyDown(Keys.W)) dy -= 5; // Move up
            // if (keyboardState.IsKeyDown(Keys.S)) dy += 5; // Move down
            if (keyboardState.IsKeyDown(Keys.A))
            {
                dx -= 5; // Move left
                _player.State = PlayerState.Running;
                _player.direction = "left";
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                dx += 5; // Move right
                _player.State = PlayerState.Running;
                _player.direction = "right";
            } 

            _player.Move(dx, dy);
        }

        private void drawGround(int startingX)
        {
            _spriteBatch.Draw(
                landTexture,
                destinationRectangle: new Rectangle(startingX, 290, 500, 500),
                color: Color.White
            );
        }    

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _player = new Player(x: 150, y: _worldBounds.Height - 80);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            backgroundTexture = Content.Load<Texture2D>("Summer3");
            spriteSheetIdle = Content.Load<Texture2D>("IDLE");
            spriteSheetRun = Content.Load<Texture2D>("RUN");
            landTexture = Content.Load<Texture2D>("4");

        }   

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            HandlePlayerInput();
            
            // TODO: Add your drawing code here
            
            _spriteBatch.Begin();
            //Player sprite
            _spriteBatch.Draw(
                backgroundTexture,
                destinationRectangle: new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                color: Color.White
            );
            drawGround(0);
            drawGround(500);
            drawGround(1000);
            
            int totalFrames;

            switch (_player.State)
            {
                case PlayerState.Idle:
                    currentSprite = spriteSheetIdle;
                    totalFrames = 10;
                    break;
                case PlayerState.Running:
                    currentSprite = spriteSheetRun;
                    totalFrames = 16;
                    break;
                // case PlayerState.Jumping:
                //     currentSprite = spriteJump;
                //     totalFrames = 2;
                //     break;
                default:
                    currentSprite = spriteSheetIdle;
                    totalFrames = 10;
                    break;
            }
            SpriteEffects effects;
            if (_player.direction == "right")
                effects = SpriteEffects.None;
            else
                effects = SpriteEffects.FlipHorizontally;

            int frameWidth = currentSprite.Width / totalFrames;
            int frameHeight = currentSprite.Height;

            int currentFrame = (int)(gameTime.TotalGameTime.TotalSeconds * 12) % totalFrames;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            _spriteBatch.Draw(currentSprite, new Rectangle(_player.Coordinates.X - 75, _player.Coordinates.Y - 105, 180, 200), sourceRect, Color.White, 0f, Vector2.Zero, effects, 0f);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
