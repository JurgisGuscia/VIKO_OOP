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
            var keyboardState = Keyboard.GetState();
            int dx = 0;
            int dy = 0;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (_player.State != PlayerState.Jumping)
                {
                    _player.State = PlayerState.Jumping;
                    _player.Velocity.ResetSpeed();
                    _player.Velocity.AddVelocity(new(0, -15f));
                }
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                dx -= 5; // Move left
                if(_player.State != PlayerState.Jumping)
                    _player.State = PlayerState.Running;
                _player.Direction = "left";
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                dx += 5; // Move right
                if(_player.State != PlayerState.Jumping)
                    _player.State = PlayerState.Running;
                _player.Direction = "right";
            }

            _player.Walk(dx, dy);
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
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, x: 600, y: 100, 30, 70);



            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }   

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (_player.Position.Coords.Y < worldBounds.WorldEndingBounds.Y - 51)
            {
                _player.Velocity.AddVelocity(new(0, 0.5f));
            }
            else
            {
                _player.Velocity.ResetSpeed();
            }
            _player.Update();


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

            PlayerVector playerBoxStart = _player.Size.StartPoint(_player.Position.Coords);

            _spriteBatch.DrawString(font, "Speed X " + (int)Math.Round(_player.Velocity.Speed.X), new Vector2(10, 30), Color.White);
            _spriteBatch.DrawString(font, "Speed Y " + (int)Math.Round(_player.Velocity.Speed.Y), new Vector2(10, 50), Color.White);
            _spriteBatch.DrawString(font, "Velocity X " + (int)Math.Round(_player.Velocity.Velocity.X), new Vector2(10, 70), Color.White);
            _spriteBatch.DrawString(font, "Velocity Y " + (int)Math.Round(_player.Velocity.Velocity.Y), new Vector2(10, 90), Color.White);

            _spriteBatch.DrawString(font, "State " + _player.State, new Vector2(10, 110), Color.White);

            // _spriteBatch.Draw(_pixel, new Rectangle((int)Math.Round(playerBoxStart.X), (int)Math.Round(playerBoxStart.Y), (int)Math.Round(_player.Size.ObjectSize.X), (int)Math.Round(_player.Size.ObjectSize.Y)), Color.Red);


            
            _player.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
