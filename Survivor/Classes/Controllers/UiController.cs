using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core;

namespace Survivor.Classes.Controllers
{
    public class UI
    {
        private SpriteBatch _spriteBatch;
        public UI(SpriteBatch spriteBatch) => _spriteBatch = spriteBatch;

        public void DrawGameInfo(SpriteFont _font, Texture2D _pixel, int gameLevel, Player player)
        {
            _spriteBatch.DrawString(_font, "Coordinates " + player.Position.Position.X + " " + player.Position.Position.Y, new Vector2(10, 300), Color.White);
            _spriteBatch.DrawString(_font, "Acceleration X " + (int)Math.Round(player.Velocity.Acceleration.X), new Vector2(10, 330), Color.White);
            _spriteBatch.DrawString(_font, "Acceleration Y " + (int)Math.Round(player.Velocity.Acceleration.Y), new Vector2(10, 350), Color.White);
            _spriteBatch.DrawString(_font, "Velocity X " + (int)Math.Round(player.Velocity.Velocity.X), new Vector2(10, 370), Color.White);
            _spriteBatch.DrawString(_font, "Velocity Y " + (int)Math.Round(player.Velocity.Velocity.Y), new Vector2(10, 390), Color.White);
            _spriteBatch.DrawString(_font, "State " + player.State, new Vector2(10, 410), Color.White);
            //draw health
            _spriteBatch.Draw(_pixel, new Rectangle(400, 50, player.Health * 5, 20), Color.Red);
            _spriteBatch.DrawString(_font, "HP:" + player.Health, new Vector2(410, 52), Color.White);
            //draw level and score
            _spriteBatch.DrawString(_font, "Level: " + gameLevel, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, "Score: " + player.Score, new Vector2(10, 30), Color.White);
        }

        public void DrawWorld(Texture2D backgroundTexture, GraphicsDevice graphicsDevice) =>
            _spriteBatch.Draw(backgroundTexture, destinationRectangle: new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), color: Color.White);

        public void DrawGround(Texture2D landTexture)
        {
            for (int i = 0; i < 1001; i += 500)
                _spriteBatch.Draw(landTexture, destinationRectangle: new Rectangle(i, 290, 500, 500), color: Color.White);
        }

        public void DrawGameOverScreen(SpriteFont font, int gameLevel, int gameResetTimer, Player player)
        {
            _spriteBatch.DrawString(font, "You lost", new Vector2(600, 300), Color.Red);
            _spriteBatch.DrawString(font, "Score: " + player.Score, new Vector2(600, 320), Color.Red);
            _spriteBatch.DrawString(font, "Level reached: " + gameLevel, new Vector2(600, 340), Color.Red);
            _spriteBatch.DrawString(font, "Game will restart in : " + (gameResetTimer / 100), new Vector2(600, 360), Color.Red);
        }
    }
}