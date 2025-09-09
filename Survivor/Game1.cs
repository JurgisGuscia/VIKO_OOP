using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlayerVector = System.Numerics.Vector2;
using Vector2 = System.Numerics.Vector2;
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
        Texture2D landTexture;
        private Texture2D _pixel;

        private SpriteFont font;
        private WorldBounds worldBounds;
        private Player _player; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            worldBounds = new WorldBounds();
            
            _graphics.PreferredBackBufferWidth = worldBounds.WorldWidth; 
            _graphics.PreferredBackBufferHeight = worldBounds.WorldHeight;
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
                _player.Direction = "left";
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                dx += 5; // Move right
                _player.State = PlayerState.Running;
                _player.Direction = "right";
            }
            
            _player.Move(dx, dy);
            _player.HandleOutOfBounds();
            
        }

        private void DrawGround(int startingX)
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
            font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, x: 600, y: worldBounds.WorldHeight - 60, 30, 70);



            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
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
        
            _spriteBatch.Draw(
                backgroundTexture,
                destinationRectangle: new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                color: Color.White
            );
            DrawGround(0);
            DrawGround(500);
            DrawGround(1000);
            
            _spriteBatch.DrawString(font, "Coordinates " + _player.Position.Coords.X + " " +  _player.Position.Coords.Y, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, "checkValue " + _player.Size.ObjectSize.X, new Vector2(10, 30), Color.White);

            PlayerVector playerBoxStart = _player.Size.StartPoint(_player.Position.Coords);
            PlayerVector playerBoxEnd = _player.Size.EndPoint(_player.Position.Coords);
            

            _spriteBatch.DrawString(font, "Start x " + (int)Math.Round(playerBoxStart.X), new Vector2(10, 50), Color.White);
            _spriteBatch.DrawString(font, "Start y " + (int)Math.Round(playerBoxStart.Y), new Vector2(10, 70), Color.White);

            _spriteBatch.Draw(_pixel, new Rectangle((int)Math.Round(playerBoxStart.X), (int)Math.Round(playerBoxStart.Y), (int)Math.Round(_player.Size.ObjectSize.X), (int)Math.Round(_player.Size.ObjectSize.Y)), Color.Red);
            _player.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
