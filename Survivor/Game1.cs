using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Survivor.Classes.Core;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Core.Interfaces;
using Survivor.Classes.Controllers;
namespace Survivor
{
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private float attackTimer = 0f;
        private Texture2D _pixel;
        private int _gameLevel = 1;
        private int _invulnerabilityTimer = 24;
        private int _invulnerabilityTimeLeft = 0;
        private SpriteFont font;

        private IWorldBounds _worldBounds;
        private WorldBoundsController _worldBoundsController;

        private Player _player;
        private static int _enemyCount = 100;
        private int _spawnedEnemies = 0;
        private int _enemySpawnPerCycle = 5;
        private int _enemySpawnCountDown = 0;
        private int _enemySpawnClock = 100;
        public int animationTime = 0;
        public bool gamePaused = false;
        public int gameResetTimer = 500;
        public Enemy[] enemies = new Enemy[_enemyCount];
        public Vector2 damageZoneStart;
        public Vector2 damageZoneEnd;

        Texture2D backgroundTexture;
        Texture2D landTexture;

        Song backgroundSong;
        SoundEffectInstance run;
        SoundEffectInstance jump;
        SoundEffectInstance land;
        SoundEffectInstance swing;
        SoundEffect ambienceEffect;

        Random random = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _worldBounds = new WorldBounds();
            _worldBoundsController = new WorldBoundsController(_worldBounds);
            _graphics.PreferredBackBufferWidth = (int)_worldBounds.WorldEnd.X;
            _graphics.PreferredBackBufferHeight = (int)_worldBounds.WorldEnd.Y;
            _graphics.ApplyChanges();

        }

        private void HandlePlayerInput(GameTime gameTime)
        {
            List<InputState> inputs = InputController.GetInput();
            int dx = 0;
            int dy = 0;
            //Handle walking actions
            if (inputs.Contains(InputState.Jump))
                if (_player.State != State.Jumping)
                {
                    jump.Play();
                    _player.SetState(State.Jumping);
                    _player.Velocity.ResetVelocity();
                    _player.Velocity.ApplyForce(new(0, -15f));
                }

            if (inputs.Contains(InputState.MoveLeft))
            {
                dx -= 5; // Move left
                if (_player.State != State.Jumping)
                    _player.SetState(State.Running);
                _player.Direction = "left";
            }

            if (inputs.Contains(InputState.MoveRight))
            {
                dx += 5; // Move right
                if (_player.State != State.Jumping)
                    _player.SetState(State.Running);
                _player.Direction = "right";
            }

            //Prevent second attack animation, before first finishes
            if (attackTimer > 0)
            {
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.SetState(State.Attacking);
            }
            //handle attack input
            else if (inputs.Contains(InputState.Attack))
            {
                if (animationTime == 0)
                {
                    attackTimer = 0.3f;
                    _player.SetState(State.Attacking);
                    animationTime = 30;
                    swing.Play();

                    if (_player.Direction == "right")
                    {
                        damageZoneStart = new(_player.Position.Position.X, _player.Position.Position.Y - _player.Size.Size.Y / 2);
                        damageZoneEnd = new(_player.Position.Position.X + 100, _player.Position.Position.Y + _player.Size.Size.Y / 2);
                    }
                    else
                    {
                        damageZoneStart = new(_player.Position.Position.X - 100, _player.Position.Position.Y - _player.Size.Size.Y / 2);
                        damageZoneEnd = new(_player.Position.Position.X, _player.Position.Position.Y + _player.Size.Size.Y /2);
                    }

                    for (int i = 0; i < _enemyCount; i++)
                    {
                        if (enemies[i] != null)
                        {
                            if (enemies[i].Position.Position.X > damageZoneStart.X && enemies[i].Position.Position.X < damageZoneEnd.X &&
                                    enemies[i].Position.Position.Y > damageZoneStart.Y && enemies[i].Position.Position.Y < damageZoneEnd.Y)
                                enemies[i].SetState(State.Dead);
                        }
                    }
                }
            }
            //move player
            _player.Walk(dx, dy);
            _player.Position.SetPosition(_worldBoundsController.PushToWorldBounds(_player.Position.Position, _player.Size.Size));
        }

        private void ConstructGround(int startingX) => _spriteBatch.Draw(landTexture, destinationRectangle: new Rectangle(startingX, 290, 500, 500), color: Color.White);

        private void DrawGround()
        {
            ConstructGround(0);
            ConstructGround(500);
            ConstructGround(1000);
        }

        protected override void Initialize() => base.Initialize();

        public void LoadSound(ref SoundEffectInstance sound, string song, bool repeat, float volume, float pitch)
        {
            ambienceEffect = Content.Load<SoundEffect>(song);
            sound = ambienceEffect.CreateInstance();
            sound.IsLooped = repeat;
            sound.Volume = volume;
            sound.Pitch = pitch;
        }

        public void FillEnemies()
        {
            var drawData = new Animator.DrawData(
                Content.Load<Texture2D>("zombie_idle"), 8,
                Content.Load<Texture2D>("zombie_walk"), 8,
                Content.Load<Texture2D>("zombie_idle"), 7,
                Content.Load<Texture2D>("zombie_death"), 5,
                Content.Load<Texture2D>("zombie_idle"), 8,
                new Vector2(50, 65),
                new Vector2(100, 100)
            );

            var boxSize = new Vector2(32, 70);
            int enemyCount = _gameLevel * 10;
            if (_enemySpawnCountDown < 1)
            {
                for (int j = 0; j < _enemySpawnPerCycle; j++)
                    if (_spawnedEnemies < enemyCount)
                        for (int i = 0; i < _enemyCount; i++)
                            if (enemies[i] == null)
                            {
                                int XPosition;
                                int pickSide = random.Next(1, 3);

                                if (pickSide == 1)
                                    XPosition = random.Next((int)Math.Round(_worldBounds.WorldEnd.X * 0.05), (int)Math.Round(_worldBounds.WorldEnd.X * 0.2));
                                else
                                    XPosition = random.Next((int)Math.Round(_worldBounds.WorldEnd.X * 0.8), (int)Math.Round(_worldBounds.WorldEnd.X * 0.95));

                                int YPosition = random.Next(40, (int)Math.Round(_worldBounds.WorldEnd.Y) - 200);
                                Vector2 position = new(XPosition, YPosition);
                                enemies[i] = new Enemy(_worldBounds, drawData, boxSize, position);
                                _spawnedEnemies++;
                                break;
                            }
                _enemySpawnCountDown = _enemySpawnClock;
            }
            else
                _enemySpawnCountDown--;
        }

        public void ResetEnemies()
        {
            for (int i = 0; i < _enemyCount; i++)
                enemies[i] = null;
        }

        public void ConstructPlayer()
        {
            var drawData = new Animator.DrawData(
                Content.Load<Texture2D>("IDLE"), 10,
                Content.Load<Texture2D>("RUN"), 16,
                Content.Load<Texture2D>("attack"), 7,
                Content.Load<Texture2D>("IDLE"), 10,
                Content.Load<Texture2D>("IDLE"), 10,
                new Vector2(92, 135),
                new Vector2(180, 200)
            );
            var boxSize = new Vector2(28, 67);
            var playerPosition = new Vector2(600, (int)Math.Round(_worldBounds.WorldEnd.Y) - 600);
            _player = new Player(_worldBounds, drawData, playerPosition, boxSize);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //create a player
            backgroundTexture = Content.Load<Texture2D>("Summer3");
            landTexture = Content.Load<Texture2D>("4");
            font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font

            ConstructPlayer();
            FillEnemies();

            //initiate background music loop
            backgroundSong = Content.Load<Song>("Mission");
            MediaPlayer.IsRepeating = true;   // loop automatically
            MediaPlayer.Volume = 0.05f;        // 0..1
            MediaPlayer.Play(backgroundSong);

            //load player sounds
            LoadSound(ref run, "step", false, 0.2f, -0.2f);
            LoadSound(ref jump, "jump", false, 0.2f, -0.2f);
            LoadSound(ref land, "land", false, 0.2f, -0.2f);
            LoadSound(ref swing, "swing", false, 0.2f, -0.2f);

            //create pixel for text drawing
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public bool CheckLevelUp()
        {
            if (_spawnedEnemies >= _gameLevel * 10)
            {
                for (int i = 0; i < _enemyCount; i++)
                    if (enemies[i] != null)
                        return false;
            }
            else
                return false;
            return true;
        }

        public bool CheckCollision(Vector2[] playerBodyCorners, Vector2 enemyBodyStart, Vector2 enemyBodyEnd)
        {
            for (int i = 0; i < 4; i++)
                if (playerBodyCorners[i].X > enemyBodyStart.X && playerBodyCorners[i].X < enemyBodyEnd.X &&
                    playerBodyCorners[i].Y > enemyBodyStart.Y && playerBodyCorners[i].Y < enemyBodyEnd.Y)
                    return true;

            return false;
        }

        public void CheckDamageTaken()
        {
            int damageTaken = 0;
            if (_invulnerabilityTimeLeft < 1)
            {
                int XOffset = (int)_player.Size.Size.X / 2;
                int YOffset = (int)_player.Size.Size.Y / 2;
                Vector2 playerTopLeft = new(_player.Position.Position.X - XOffset, _player.Position.Position.Y - YOffset);
                Vector2 playerTopRight = new(_player.Position.Position.X + XOffset, _player.Position.Position.Y - YOffset);
                Vector2 playerBottomLeft = new(_player.Position.Position.X - XOffset, _player.Position.Position.Y + YOffset);
                Vector2 PlayerBottomRight = new(_player.Position.Position.X + XOffset, _player.Position.Position.Y + YOffset);
                Vector2[] playerBodyCorners = [playerTopLeft, playerTopRight, playerBottomLeft, PlayerBottomRight];

                for (int i = 0; i < _enemyCount; i++)
                    if (enemies[i] != null)
                    {
                        XOffset = (int)enemies[i].Size.Size.X / 2;
                        YOffset = (int)enemies[i].Size.Size.Y / 2;
                        Vector2 enemyBodyStart = new(enemies[i].Position.Position.X - XOffset, enemies[i].Position.Position.Y - YOffset);
                        Vector2 enemyBodyEnd = new(enemies[i].Position.Position.X + XOffset, enemies[i].Position.Position.Y + YOffset);

                        if (CheckCollision(playerBodyCorners, enemyBodyStart, enemyBodyEnd))
                            damageTaken++;
                    }
                //if damage taken, give player temporal invulnerability
                if (damageTaken > 0)
                    _invulnerabilityTimeLeft = _invulnerabilityTimer;

                _player.TakeDamage(damageTaken);
            }
        }

        public void resetGame()
        {
            //reset game world
            _gameLevel = 1;
            gameResetTimer = 500;
            gamePaused = false;
            _spawnedEnemies = 0;
            ConstructPlayer();
            //reset enemies
            ResetEnemies();
            FillEnemies();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //if game is paused, don't run update
            if (!gamePaused)
            {
                //if player is off ground, apply gravity
                if (_player.Position.Position.Y <= _worldBounds.WorldEnd.Y - _player.Size.Size.Y / 2 - 20)
                    _player.Velocity.ApplyForce(new(0, 0.5f));
                else
                    if(_player.Velocity.Velocity.Y > 0)
                        _player.Velocity.ResetAccelerationY();
                
                    
                _player.Update(_player.Position.Position);
                _player.Position.SetPosition(_worldBoundsController.PushToWorldBounds(_player.Position.Position, _player.Size.Size));

                for (int i = 0; i < _enemyCount; i++)
                    if (enemies[i] != null)
                    {
                        //if enemy is off ground, apply gravity
                        if (enemies[i].Position.Position.Y < _worldBounds.WorldEnd.Y - enemies[i].Size.Size.Y / 2)
                            enemies[i].Velocity.ApplyForce(new(0, 0.5f));
                        else
                            enemies[i].Velocity.ResetAccelerationY();

                        //if enemy is dead, add score and remove enemy from array
                        if (enemies[i].State == State.Dead && enemies[i].AnimationFinished())
                        {
                            enemies[i] = null;
                            _player.AddScore(_gameLevel * 10);
                        }
                        else
                        {
                            //if enemy alive, move it
                            enemies[i].Update(_player.Position.Position);
                            enemies[i].Position.SetPosition(_worldBoundsController.PushToWorldBounds(enemies[i].Position.Position, enemies[i].Size.Size));
                        }
                    }

                //check if player took damage
                CheckDamageTaken();
                //check if should level up
                if (CheckLevelUp())
                {
                    _gameLevel++;
                    _spawnedEnemies = 0;
                }
                else
                    FillEnemies();
                if (_player.Health < 1)
                    gamePaused = true;
                //reduce invulnerability timer
                _invulnerabilityTimeLeft--;
            }
            else
            {
                if (gameResetTimer < 1)
                    resetGame();
                else
                    gameResetTimer--;

            }
            base.Update(gameTime);

        }

        public void DrawGameInfo()
        {
            //draw debug info
            _spriteBatch.DrawString(font, "Coordinates " + _player.Position.Position.X + " " + _player.Position.Position.Y, new Vector2(10, 300), Color.White);
            _spriteBatch.DrawString(font, "Acceleration X " + (int)Math.Round(_player.Velocity.Acceleration.X), new Vector2(10, 330), Color.White);
            _spriteBatch.DrawString(font, "Acceleration Y " + (int)Math.Round(_player.Velocity.Acceleration.Y), new Vector2(10, 350), Color.White);
            _spriteBatch.DrawString(font, "Velocity X " + (int)Math.Round(_player.Velocity.Velocity.X), new Vector2(10, 370), Color.White);
            _spriteBatch.DrawString(font, "Velocity Y " + (int)Math.Round(_player.Velocity.Velocity.Y), new Vector2(10, 390), Color.White);
            _spriteBatch.DrawString(font, "State " + _player.State, new Vector2(10, 410), Color.White);
            _spriteBatch.DrawString(font, "HitBox start X" + (int)Math.Round(damageZoneStart.X), new Vector2(10, 430), Color.White);
            _spriteBatch.DrawString(font, "HitBox start Y" + (int)Math.Round(damageZoneStart.Y), new Vector2(10, 450), Color.White);
            _spriteBatch.DrawString(font, "HitBox end X" + (int)Math.Round(damageZoneEnd.X), new Vector2(10, 470), Color.White);
            _spriteBatch.DrawString(font, "HitBox end Y" + (int)Math.Round(damageZoneEnd.Y), new Vector2(10, 490), Color.White);
            //draw health
            _spriteBatch.Draw(_pixel, new Rectangle(400, 50, _player.Health * 5, 20), Color.Red);
            _spriteBatch.DrawString(font, "HP:" + _player.Health, new Vector2(410, 52), Color.White);
            //draw level and score
            _spriteBatch.DrawString(font, "Level: " + _gameLevel, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(10, 30), Color.White);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (!gamePaused)
            {
                HandlePlayerInput(gameTime);
                if (_player.State == State.Running)
                    run.Play();
                else
                    run.Pause();

                if (_player.State == State.Jumping && _player.Position.Position.Y > _worldBounds.WorldEnd.Y - 51 && _player.Velocity.Acceleration.Y > 0)
                    land.Play();

                _spriteBatch.Begin();

                _spriteBatch.Draw(
                    backgroundTexture,
                    destinationRectangle: new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                    color: Color.White
                );
                DrawGround();
                DrawGameInfo();

                _player.Draw(_spriteBatch, gameTime);

                foreach (Enemy enemy in enemies)
                    if (enemy != null)
                    {
                        _worldBoundsController.PushToWorldBounds(enemy.Position.Position, enemy.Size.Size);
                        enemy.Draw(_spriteBatch, gameTime);
                    }

                _spriteBatch.End();
                if (animationTime > 0)
                    animationTime--;
            }
            else
            {
                run.Pause();
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    backgroundTexture,
                    destinationRectangle: new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                    color: Color.White
                );
                DrawGround();
                DrawGameInfo();

                _spriteBatch.DrawString(font, "You lost", new Vector2(600, 300), Color.Red);
                _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(600, 320), Color.Red);
                _spriteBatch.DrawString(font, "Level reached: " + _gameLevel, new Vector2(600, 340), Color.Red);
                _spriteBatch.DrawString(font, "Game will restart in : " + (gameResetTimer / 100), new Vector2(600, 360), Color.Red);

                _spriteBatch.End();

            }
            base.Draw(gameTime);

        }
    }
}
