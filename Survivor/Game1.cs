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
using System.Reflection.Emit;
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
        private SpriteFont _font;
        private UI _ui;
        private IWorldBounds _worldBounds;
        private WorldBoundsController _worldBoundsController;
        private EnemyController _enemyController;

        private Player _player;
        private int _enemySpawnCountDown = 0;
        private int _enemySpawnClock = 100;
        public int animationTime = 0;
        public bool gamePaused = false;
        public int gameResetTimer = 500;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _worldBounds = new WorldBounds();
            _worldBoundsController = new WorldBoundsController(_worldBounds);
            int maxEnemyCount = 100;
            int enemySpawnsPerCycle = 5;
            _enemyController = new EnemyController(maxEnemyCount, _worldBounds, enemySpawnsPerCycle);
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
                        damageZoneStart = new(_player.Position.Position.X, _player.Position.Position.Y - _player.Size.Size.Y / 2 - 30);
                        damageZoneEnd = new(_player.Position.Position.X + 100, _player.Position.Position.Y + _player.Size.Size.Y / 2 +30);
                    }
                    else
                    {
                        damageZoneStart = new(_player.Position.Position.X - 100, _player.Position.Position.Y - _player.Size.Size.Y / 2 + 30);
                        damageZoneEnd = new(_player.Position.Position.X, _player.Position.Position.Y + _player.Size.Size.Y / 2+ 30);
                    }

                    _enemyController.KillEnemies(damageZoneStart, damageZoneEnd);

                }
            }
            //move player
            _player.Walk(dx, dy);
            _player.Position.SetPosition(_worldBoundsController.PushToWorldBounds(_player.Position.Position, _player.Size.Size));
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
            var enemyDrawData = new Animator.DrawData(
                Content.Load<Texture2D>("zombie_idle"), 8,
                Content.Load<Texture2D>("zombie_walk"), 8,
                Content.Load<Texture2D>("zombie_idle"), 7,
                Content.Load<Texture2D>("zombie_death"), 5,
                Content.Load<Texture2D>("zombie_idle"), 8,
                new Vector2(50, 65),
                new Vector2(100, 100)
            );
            int enemyLimitThisLevel = _gameLevel * 10;
            Vector2 enemyBoxSize = new(32, 70);
            if (_enemySpawnCountDown < 1)
            {
                _enemyController.FillEnemies(enemyLimitThisLevel, enemyDrawData, enemyBoxSize);
                _enemySpawnCountDown = _enemySpawnClock;
            }
            else
                _enemySpawnCountDown--;
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
            _font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font

            ConstructPlayer();
            FillEnemies();
            _ui = new UI(_spriteBatch);
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
            if (_enemyController.ShouldLevelUp(_gameLevel))
                return true;
            else
                return false;
        }

        public void CheckDamageTaken()
        {
            if (_invulnerabilityTimeLeft < 1)
            {
                int damageTaken = _enemyController.CalculateCollisions(_player);    
                //if damage taken, give player temporal invulnerability
                if (damageTaken > 0)
                    _invulnerabilityTimeLeft = _invulnerabilityTimer;
                _player.TakeDamage(damageTaken);
            }
        }

        public void ResetGame()
        {
            //reset game world
            _gameLevel = 1;
            gameResetTimer = 500;
            gamePaused = false;
            _player = null;
            ConstructPlayer();
            _enemyController.ResetEnemies();
            _enemyController.ResetSpawnedEnemies();
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

                _enemyController.UpdateEnemies(_worldBoundsController, _gameLevel, _player);

                //check if player took damage
                CheckDamageTaken();
                //check if should level up
                if (CheckLevelUp())
                {
                    _gameLevel++;
                    _enemyController.ResetSpawnedEnemies();
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
                    ResetGame();
                else
                    gameResetTimer--;

            }
            base.Update(gameTime);

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

                _ui.DrawWorld(backgroundTexture, GraphicsDevice);
                _ui.DrawGround(landTexture);
                _ui.DrawGameInfo(_font, _pixel, _gameLevel, _player);

                _player.Draw(_spriteBatch, gameTime);

                _enemyController.DrawEnemies(_worldBoundsController, _spriteBatch, gameTime);

                _spriteBatch.End();
                if (animationTime > 0)
                    animationTime--;
            }
            else
            {
                _spriteBatch.Begin();
                _ui.DrawWorld(backgroundTexture, GraphicsDevice);
                _ui.DrawGround(landTexture);
                _ui.DrawGameInfo(_font, _pixel, _gameLevel, _player);
                _ui.DrawGameOverScreen(_font, _gameLevel, gameResetTimer, _player);
                _spriteBatch.End();

            }
            base.Draw(gameTime);

        }
    }
}
