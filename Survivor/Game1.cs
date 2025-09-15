using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Survivor.Classes.Core;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Core.Interfaces;
using Survivor.Classes.Controllers;
using System.Collections.Generic;
namespace Survivor
{
    public partial class  Game1 : Game
    {
        private int _gameLevel = 1;
        private int _invulnerabilityTimer = 24;
        private int _invulnerabilityTimeLeft = 0;
        private float attackTimer = 0f;
        private int _enemySpawnCountDown = 0;
        private int _enemySpawnClock = 100;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;
        private UI _ui;
        private IWorldBounds _worldBounds;
        private WorldBoundsController _worldBoundsController;
        private EnemyController _enemyController;
        private Player _player;
        private EnemyDropController _dropController;
        private WorldController _worldController;
        private EffectController _effectController;
        private FireBallController _fireBallController;
        
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
        SoundEffectInstance fireball;
        SoundEffect ambienceEffect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _worldBounds = new WorldBounds();
            _worldBoundsController = new WorldBoundsController(_worldBounds);
            _dropController = new EnemyDropController();
            _worldController = new WorldController();
            int maxEnemyCount = 100;
            int enemySpawnsPerCycle = 5;
            _enemyController = new EnemyController(maxEnemyCount, _worldBounds, enemySpawnsPerCycle);
            _effectController = new EffectController();
            
            
            _graphics.PreferredBackBufferWidth = (int)_worldBounds.WorldEnd.X;
            _graphics.PreferredBackBufferHeight = (int)_worldBounds.WorldEnd.Y;
            _graphics.ApplyChanges();

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

        protected override void LoadContent()
        {
            //load textures
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>("Summer3");
            landTexture = Content.Load<Texture2D>("4");
            _font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font
            _ui = new UI(_spriteBatch);//initialize UI controller
            //initiate background music loop
            backgroundSong = Content.Load<Song>("Mission");
            MediaPlayer.IsRepeating = true;   // loop automatically
            MediaPlayer.Volume = 0.35f;        // 0..1
            MediaPlayer.Play(backgroundSong);

            //load player sounds
            LoadSound(ref run, "step", false, 0.2f, -0.2f);
            LoadSound(ref jump, "jump", false, 0.2f, -0.2f);
            LoadSound(ref land, "land", false, 0.2f, -0.2f);
            LoadSound(ref swing, "swing", false, 0.2f, -0.2f);
            LoadSound(ref fireball, "fireBallSound", false, 0.2f, -0.2f);

            //create pixel for text drawing
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            //create player and enemies
            ConstructPlayer();
            FillEnemies();
            
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (!gamePaused)//if game over don't update
            {
                if (_player.Position.Position.Y <= _worldBounds.WorldEnd.Y - _player.Size.Size.Y / 2 - 20)//if player is off ground, apply gravity
                    _player.Velocity.ApplyForce(new(0f, _worldController.ApplyGravity));
                else
                    if(_player.Velocity.Velocity.Y > 0)
                        _player.Velocity.ResetAccelerationY();
                    
                _player.Update(_player.Position.Position);
                _player.Position.SetPosition(_worldBoundsController.PushToWorldBounds(_player.Position.Position, _player.Size.Size));
                if (_worldController.PushFromPlayer)
                    _enemyController.PushEnemies(_player.Position.Position, _worldController.ApplyPushForce);

                _enemyController.UpdateEnemies(_worldBoundsController, _gameLevel, _player);
                _dropController.Update(_player.Position.Position);
                _dropController.HandlePickedUpItems(_player.Position.Position, _worldController);
                _player.AddHealth(_dropController.GetItemHeal());
                _player.AddScore(_dropController.GetItemScore());
                _effectController.UpdateEffectList();
                _worldController.UpdateWorldEffects();
                if (_fireBallController != null)
                {
                    _fireBallController.Update(new(0, 0));
                    Vector2 StartPoint = new(_fireBallController.Position.Position.X - 20, _fireBallController.Position.Position.Y - 20);
                    Vector2 EndPoint = new(_fireBallController.Position.Position.X + 20, _fireBallController.Position.Position.Y + 20);
                    _enemyController.KillEnemies(StartPoint, EndPoint);
                    if (!FireballStillActive())
                    {
                        _fireBallController = null;
                        fireball.Stop();
                    }
                        
                }

                if (_worldController.BurnEnemies)
                {
                    List<Vector2> DropSpawnLocations = _enemyController.KillEnemies(new(0, _worldBounds.WorldEnd.Y - 40), new(_worldBounds.WorldEnd.X, _worldBounds.WorldEnd.Y));
                    foreach (Vector2 effect in DropSpawnLocations)
                    {
                        LoadAndAddEffect(effect, State.Attacking);
                    }

                    LoadDropDataAndGenerateDrops(DropSpawnLocations);
                }

                


                CheckDamageTaken();//check if player took damage
                if (CheckLevelUp())//check if should level up
                {
                    _gameLevel++;
                    _enemyController.ResetSpawnedEnemies();
                }
                else
                    FillEnemies();
                if (_player.Health < 1)//check for game over    
                    gamePaused = true;
                _invulnerabilityTimeLeft--;//reduce invulnerability timer
            }
            else
                if (gameResetTimer < 1)
                    ResetGame();
                else
                    gameResetTimer--;
            
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (!gamePaused)//if game ir running draw fully
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
                _ui.DrawGameInstructions(_font, LoadItemData(), _spriteBatch);
                _player.Draw(_spriteBatch, gameTime);
                _dropController.DrawDrops(_spriteBatch, gameTime);
                _enemyController.DrawEnemies(_worldBoundsController, _spriteBatch, gameTime);
                _effectController.DrawEffects(_spriteBatch, gameTime);
                if (_fireBallController != null)
                    _fireBallController.Draw(_spriteBatch, gameTime);
                _spriteBatch.End();
                if (animationTime > 0)
                    animationTime--;
            }
            else//if game over draw game over screen
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
