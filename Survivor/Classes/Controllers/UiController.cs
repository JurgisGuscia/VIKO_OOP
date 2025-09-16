using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core;

namespace Survivor.Classes.Controllers
{
    public class UI
    {
        private SpriteBatch _spriteBatch;
        private int _currentCoinFrame = 0;
        private int _currentRuneFrame = 0;
        public UI(SpriteBatch spriteBatch) => _spriteBatch = spriteBatch;
        public void DrawGameInfo(SpriteFont _font, Texture2D _pixel, int gameLevel, Player player)
        {
            //debug parameters
            // _spriteBatch.DrawString(_font, "Coordinates " + player.Position.Position.X + " " + player.Position.Position.Y, new Vector2(10, 300), Color.White);
            // _spriteBatch.DrawString(_font, "Acceleration X " + (int)Math.Round(player.Velocity.Acceleration.X), new Vector2(10, 330), Color.White);
            // _spriteBatch.DrawString(_font, "Acceleration Y " + (int)Math.Round(player.Velocity.Acceleration.Y), new Vector2(10, 350), Color.White);
            // _spriteBatch.DrawString(_font, "Velocity X " + (int)Math.Round(player.Velocity.Velocity.X), new Vector2(10, 370), Color.White);
            // _spriteBatch.DrawString(_font, "Velocity Y " + (int)Math.Round(player.Velocity.Velocity.Y), new Vector2(10, 390), Color.White);
            // _spriteBatch.DrawString(_font, "State " + player.State, new Vector2(10, 410), Color.White);
            _spriteBatch.Draw(_pixel, new Rectangle(400, 50, player.Health * 5, 20), Color.Red);
            _spriteBatch.DrawString(_font, "HP:" + player.Health, new Vector2(410, 51), Color.White);
            _spriteBatch.Draw(_pixel, new Rectangle(400, 75, player.Mana * 5, 20), Color.Blue);
            _spriteBatch.DrawString(_font, "MP:" + player.Mana, new Vector2(410, 77), Color.White);
            _spriteBatch.DrawString(_font, "Level: " + gameLevel, new Vector2(410, 100), Color.White);
            _spriteBatch.DrawString(_font, "Score: " + player.Score, new Vector2(500, 100), Color.White);
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

        public void DrawGameInstructions(SpriteFont _font, Animator.DrawData drawData, SpriteBatch spriteBatch)
        {
            if (_currentCoinFrame > drawData.idleFrames)
                _currentCoinFrame = 0;
            if (_currentRuneFrame > drawData.attackFrames)
                _currentRuneFrame = 0;    
            _spriteBatch.DrawString(_font, "D - move right ", new Vector2(40, 10), Color.White);
            _spriteBatch.DrawString(_font, "A - move left ", new Vector2(40, 30), Color.White);
            _spriteBatch.DrawString(_font, "W - Jump ", new Vector2(34, 50), Color.White);
            _spriteBatch.DrawString(_font, "J - Attack ", new Vector2(40, 70), Color.White);
            _spriteBatch.DrawString(_font, "K - Special attack (50 mana) ", new Vector2(38, 90), Color.White);
            _spriteBatch.DrawString(_font, " - Points ", new Vector2(50, 112), Color.White);
            _spriteBatch.DrawString(_font, " - Health ", new Vector2(50, 132), Color.White);
            _spriteBatch.DrawString(_font, " - Push enemies", new Vector2(50, 152), Color.White);
            _spriteBatch.DrawString(_font, " - Burn enemies ", new Vector2(50, 172), Color.White);

            Texture2D GoldCoinSprite = drawData.spriteSheetIdle;
            int GoldCoinFrames = drawData.idleFrames;
            Texture2D HeartSprite = drawData.spriteSheetRun;
            int HeartFrames = drawData.runFrames;
            Texture2D WhiteRuneSprite = drawData.spriteSheetAttack;
            int WhiteRuneFrames = drawData.attackFrames;
            Texture2D RedRune = drawData.spriteSheetDead;
            int RedRuneFrames = drawData.deadFrames;
            Rectangle sourceRect;

            int CoinFrameWidth = GoldCoinSprite.Width / GoldCoinFrames;
            int CoinFrameHeight = GoldCoinSprite.Height;
            sourceRect = new Rectangle(_currentCoinFrame * CoinFrameWidth, 0, CoinFrameWidth, CoinFrameHeight);
            spriteBatch.Draw(
                GoldCoinSprite,
                new Rectangle(35, 116, 15, 15),
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );

            sourceRect = new Rectangle(_currentCoinFrame * CoinFrameWidth, 0, CoinFrameWidth, CoinFrameHeight);
            spriteBatch.Draw(
                HeartSprite,
                new Rectangle(35, 136, 15, 15),
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );

            int RuneFrameWidth = WhiteRuneSprite.Width / WhiteRuneFrames;
            int RuneFrameHeight = WhiteRuneSprite.Height;
            sourceRect = new Rectangle(_currentRuneFrame * RuneFrameWidth, 0, RuneFrameWidth, RuneFrameHeight);
            spriteBatch.Draw(
                WhiteRuneSprite,
                new Rectangle(31, 151, 25, 25),
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
            
            sourceRect = new Rectangle(_currentRuneFrame * RuneFrameWidth, 0, RuneFrameWidth, RuneFrameHeight);
            spriteBatch.Draw(
                RedRune,
                new Rectangle(31, 171, 25, 25),
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
            
            _currentCoinFrame++;
            _currentRuneFrame++;
        }
    }
}