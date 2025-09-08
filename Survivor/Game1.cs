using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Survivor;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D spriteSheet;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1280;  // desired width
        _graphics.PreferredBackBufferHeight = 720;  // desired height
        _graphics.ApplyChanges();

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
        
        spriteSheet = Content.Load<Texture2D>("IDLE");
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

        int frameWidth = spriteSheet.Width / 10; // number of frames
        int frameHeight = spriteSheet.Height;
        // TODO: Add your drawing code here
        int currentFrame = (int)(gameTime.TotalGameTime.TotalSeconds * 8) % 4; // 10 fps

        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

        _spriteBatch.Begin();
        _spriteBatch.Draw(spriteSheet, new Rectangle(150, 150, 180, 200), sourceRect, Color.White);
        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
