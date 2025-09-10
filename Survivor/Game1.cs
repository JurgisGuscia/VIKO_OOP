using System;
using System.Numerics;
using Microsoft.Xna.Framework.Media;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlayerVector = System.Numerics.Vector2;
using Vector2 = System.Numerics.Vector2;
using Microsoft.Xna.Framework.Audio; 
using Survivor.Classes;
namespace Survivor
{
    public class Game1 : Game
    {
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private float attackTimer = 0f;
        private Texture2D _pixel;
        private int gameLevel = 1;
        private int invulnerabilityTimer = 24;
        private int invulnerabilityTimeLeft = 0;
        private SpriteFont font;
        private WorldBounds worldBounds;
        private Player _player;
        private static int _enemyCount = 100;

        public int animationTime = 0;
        public bool gamePaused = false;
        public int gameResetTimer = 500;
        public Enemy[] enemies = new Enemy[_enemyCount];
        public Vector2 damageZoneStart;
        public Vector2 damageZoneEnd;

        Texture2D spriteSheetIdle;
        Texture2D spriteSheetRun;
        Texture2D spriteSheetAttack;
        Texture2D spriteSheetDead;
        Texture2D backgroundTexture;
        Texture2D landTexture;

        Song backgroundSong;
        SoundEffectInstance run;
        SoundEffectInstance jump;
        SoundEffectInstance land;
        SoundEffectInstance swing;
        
        Random random = new Random();

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

        private void HandlePlayerInput(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            int dx = 0;
            int dy = 0;

            //Handle walking actions
            if (keyboardState.IsKeyDown(Keys.W))
                if (_player.State != PlayerState.Jumping)
                {
                    jump.Play();
                    _player.State = PlayerState.Jumping;
                    _player.Velocity.ResetSpeed();
                    _player.Velocity.AddVelocity(new(0, -15f));
                }
                
            if (keyboardState.IsKeyDown(Keys.A))
            {
                dx -= 5; // Move left
                if (_player.State != PlayerState.Jumping)
                    _player.State = PlayerState.Running;
                _player.Direction = "left";
            }
            
            if (keyboardState.IsKeyDown(Keys.D))
            {
                dx += 5; // Move right
                if (_player.State != PlayerState.Jumping)
                    _player.State = PlayerState.Running;
                _player.Direction = "right";
            }

            //Prevent second attack animation, before first finishes
            if (attackTimer > 0)
            {
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.State = PlayerState.Attacking;
            }
            //handle attack input
            else if (keyboardState.IsKeyDown(Keys.J))
            {
                if (animationTime == 0)
                {
                    attackTimer = 0.3f;
                    _player.State = PlayerState.Attacking;
                    animationTime = 30;
                    swing.Play();

                    if (_player.Direction == "right")
                    {
                        damageZoneStart = new(_player.Position.Coords.X, _player.Position.Coords.Y - 50);
                        damageZoneEnd = new(_player.Position.Coords.X + 90, _player.Position.Coords.Y + 50);
                    }
                    else
                    {
                        damageZoneStart = new(_player.Position.Coords.X - 90, _player.Position.Coords.Y - 100);
                        damageZoneEnd = new(_player.Position.Coords.X, _player.Position.Coords.Y + 100);
                    }

                    for (int i = 0; i < _enemyCount; i++)
                    {
                        if (enemies[i] != null)
                        {
                            if (enemies[i].Position.Coords.X > damageZoneStart.X &&
                                enemies[i].Position.Coords.X < damageZoneEnd.X &&
                                enemies[i].Position.Coords.Y > damageZoneStart.Y &&
                                enemies[i].Position.Coords.Y < damageZoneEnd.Y)
                            {
                                enemies[i].State = EnemyState.Dead;
                            }
                        }
                    }
                }
            }
            //move player
            _player.Walk(dx, dy);
            _player.HandleOutOfBounds();
        }

        private void ConstructGround(int startingX) =>  _spriteBatch.Draw(landTexture,destinationRectangle: new Rectangle(startingX, 290, 500, 500), color: Color.White);
            
        private void DrawGround()
        {
            ConstructGround(0);
            ConstructGround(500);
            ConstructGround(1000);
        }    

        protected override void Initialize() => base.Initialize();

        public void FillEnemies(int enemyCount) {
            for (int i = 0; i < enemyCount; i++)
                if (enemies[i] == null)
                {
                    int XPosition = random.Next(40, (int)Math.Round(worldBounds.WorldEndingBounds.X));
                    int YPosition = random.Next(40, (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 200);
                    enemies[i] = new Enemy(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: XPosition, y: YPosition, 30, 70);
                }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //create a player
            backgroundTexture = Content.Load<Texture2D>("Summer3");
            spriteSheetIdle = Content.Load<Texture2D>("IDLE");
            spriteSheetRun = Content.Load<Texture2D>("RUN");
            spriteSheetAttack = Content.Load<Texture2D>("attack");
            spriteSheetDead = Content.Load<Texture2D>("IDLE");
            landTexture = Content.Load<Texture2D>("4");
            font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: 600, y: (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 55, 28, 67);

            //create initial array of enemies
            spriteSheetIdle = Content.Load<Texture2D>("zombie_idle");
            spriteSheetRun = Content.Load<Texture2D>("zombie_walk");
            spriteSheetAttack = Content.Load<Texture2D>("zombie_attack");
            spriteSheetDead = Content.Load<Texture2D>("zombie_death");
            FillEnemies(gameLevel * 10);

            //initiate background music loop
            backgroundSong = Content.Load<Song>("Mission");
            MediaPlayer.IsRepeating = true;   // loop automatically
            MediaPlayer.Volume = 0.1f;        // 0..1
            MediaPlayer.Play(backgroundSong);

            //create run sound for player
            SoundEffect ambienceEffect = Content.Load<SoundEffect>("step");
            run = ambienceEffect.CreateInstance();
            run.IsLooped = true;
            run.Volume = 0.2f;
            run.Pitch = -0.2f;

            //create jump sound for player
            ambienceEffect = Content.Load<SoundEffect>("jump");
            jump = ambienceEffect.CreateInstance();
            jump.IsLooped = false;
            jump.Volume = 0.2f;
            jump.Pitch = 0f;

            //create land sound for player
            ambienceEffect = Content.Load<SoundEffect>("land");
            land = ambienceEffect.CreateInstance();
            land.IsLooped = false;
            land.Volume = 0.2f;
            land.Pitch = 0f;

            //create sword swing sound
            ambienceEffect = Content.Load<SoundEffect>("swing");
            swing = ambienceEffect.CreateInstance();
            swing.IsLooped = false;
            swing.Volume = 0.2f;
            swing.Pitch = 0f;
            
            //create pixel for text drawing
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void CheckLevelUp()
        {
            //check if all enemies are dead
            bool levelCompleted = true;
            for (int i = 0; i < _enemyCount; i++)
                if (enemies[i] != null)
                    levelCompleted = false;

            //if all enemies are dead, increase level
            if (levelCompleted)
            {
                gameLevel++;
                if (gameLevel + 10 > 100)
                    FillEnemies(100);
                else
                    FillEnemies(gameLevel + 10);
            }
        }

        public bool CheckCollision(Vector2[] playerBodyCorners, Vector2 enemyBodyStart, Vector2 enemyBodyEnd) {
            bool collision = false;

            //top left corner collision
            if (playerBodyCorners[0].X > enemyBodyStart.X && playerBodyCorners[0].X < enemyBodyEnd.X &&
                playerBodyCorners[0].Y > enemyBodyStart.Y && playerBodyCorners[0].Y < enemyBodyEnd.Y)
                collision = true;

            //top right corner collision
            if (playerBodyCorners[1].X > enemyBodyStart.X && playerBodyCorners[1].X < enemyBodyEnd.X &&
                playerBodyCorners[1].Y > enemyBodyStart.Y && playerBodyCorners[1].Y < enemyBodyEnd.Y)
                collision = true;

            //bottom left corner collision
            if (playerBodyCorners[2].X > enemyBodyStart.X && playerBodyCorners[2].X < enemyBodyEnd.X &&
                playerBodyCorners[2].Y > enemyBodyStart.Y && playerBodyCorners[2].Y < enemyBodyEnd.Y)
                collision = true;

            //bottom right corner collision
            if (playerBodyCorners[3].X > enemyBodyStart.X && playerBodyCorners[3].X < enemyBodyEnd.X &&
                playerBodyCorners[3].Y > enemyBodyStart.Y && playerBodyCorners[3].Y < enemyBodyEnd.Y)
                collision = true;

            return collision;
        }


        public void CheckDamageTaken()
        {
            int damageTaken = 0;
            if (invulnerabilityTimeLeft < 1)
            {
                Vector2 playerTopLeft = new(_player.Position.Coords.X - _player.Size.ObjectSize.X, _player.Position.Coords.Y - _player.Size.ObjectSize.Y / 2);
                Vector2 playerTopRight = new(_player.Position.Coords.X + _player.Size.ObjectSize.X, _player.Position.Coords.Y - _player.Size.ObjectSize.Y / 2);
                Vector2 playerBottomLeft = new(_player.Position.Coords.X - _player.Size.ObjectSize.X, _player.Position.Coords.Y + _player.Size.ObjectSize.Y / 2);
                Vector2 PlayerBottomRight = new(_player.Position.Coords.X + _player.Size.ObjectSize.X, _player.Position.Coords.Y + _player.Size.ObjectSize.Y / 2);
                Vector2[] playerBodyCorners = [playerTopLeft, playerTopRight, playerBottomLeft, PlayerBottomRight];

                for (int i = 0; i < _enemyCount; i++)
                    if (enemies[i] != null)
                    {
                        Vector2 enemyBodyStart = new(enemies[i].Position.Coords.X - enemies[i].Size.ObjectSize.X / 2, enemies[i].Position.Coords.Y - enemies[i].Size.ObjectSize.Y / 2);
                        Vector2 enemyBodyEnd = new(enemies[i].Position.Coords.X + enemies[i].Size.ObjectSize.X / 2, enemies[i].Position.Coords.Y + enemies[i].Size.ObjectSize.Y / 2);

                        if (CheckCollision(playerBodyCorners, enemyBodyStart, enemyBodyEnd))
                            damageTaken++;
                    }
                //if damage taken, give player temporal invulnerability
                if (damageTaken > 0)
                    invulnerabilityTimeLeft = invulnerabilityTimer;

                _player.TakeDamage(damageTaken);
            }
        }

        public void resetGame()
        {
            //reset player
            _player = null;
            spriteSheetIdle = Content.Load<Texture2D>("IDLE");
            spriteSheetRun = Content.Load<Texture2D>("RUN");
            spriteSheetAttack = Content.Load<Texture2D>("attack");
            spriteSheetDead = Content.Load<Texture2D>("IDLE");
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: 600, y: (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 55, 28, 67);

            //reset enemies
            for (int i = 0; i < _enemyCount; i++)
                enemies[i] = null;
            spriteSheetIdle = Content.Load<Texture2D>("zombie_idle");
            spriteSheetRun = Content.Load<Texture2D>("zombie_walk");
            spriteSheetAttack = Content.Load<Texture2D>("zombie_attack");
            spriteSheetDead = Content.Load<Texture2D>("zombie_death");
            FillEnemies(gameLevel * 10);

            //reset game world
            gameLevel = 1;
            gameResetTimer = 500;
            gamePaused = false;
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //if game is paused, don't run update
            if (!gamePaused)
            {
                //if player is off ground, apply gravity
                if (_player.Position.Coords.Y < worldBounds.WorldEndingBounds.Y - 51)
                    _player.Velocity.AddVelocity(new(0, 0.5f));
                else
                    _player.Velocity.ResetSpeedY();

                _player.Update(_player.Position.Coords);

                for (int i = 0; i < _enemyCount; i++)
                    if (enemies[i] != null)
                    {
                        //if enemy is off ground, apply gravity
                        if (enemies[i].Position.Coords.Y < worldBounds.WorldEndingBounds.Y - 51)
                            enemies[i].Velocity.AddVelocity(new(0, 0.5f));
                        else
                            enemies[i].Velocity.ResetSpeedY();
                        
                        //if enemy is dead, add score and remove enemy from array
                        if (enemies[i].State == EnemyState.Dead)
                            //remove enemy from array after animation plays out
                            if (enemies[i].DeathFrames == 0)
                            {
                                enemies[i] = null;
                                _player.AddScore(gameLevel * 10);
                            }
                            else
                                enemies[i].DeathFrames--;
                        else
                            //if enemy alive, move it
                            enemies[i].Update(_player.Position.Coords);
                    }
                //check if player took damage
                CheckDamageTaken();
                //check if should level up
                CheckLevelUp();
                if (_player.Health < 1)
                    gamePaused = true;

                //reduce invulnerability timer
                invulnerabilityTimeLeft--;
            }
            base.Update(gameTime);

        }

        public void DrawGameInfo()
        {
            //draw debug info
            _spriteBatch.DrawString(font, "Coordinates " + _player.Position.Coords.X + " " + _player.Position.Coords.Y, new Vector2(10, 300), Color.White);
            _spriteBatch.DrawString(font, "Speed X " + (int)Math.Round(_player.Velocity.Speed.X), new Vector2(10, 330), Color.White);
            _spriteBatch.DrawString(font, "Speed Y " + (int)Math.Round(_player.Velocity.Speed.Y), new Vector2(10, 350), Color.White);
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
            _spriteBatch.DrawString(font, "Level: " + gameLevel, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(10, 30), Color.White);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (!gamePaused)
            {
                HandlePlayerInput(gameTime);
                if (_player.State == PlayerState.Running)
                    run.Play();
                else
                    run.Pause();

                if (_player.State == PlayerState.Jumping && _player.Position.Coords.Y > worldBounds.WorldEndingBounds.Y - 51 && _player.Velocity.Speed.Y > 0)
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
                        enemy.HandleOutOfBounds();
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
                _spriteBatch.DrawString(font, "Level reached: " + gameLevel, new Vector2(600, 340), Color.Red);
                _spriteBatch.DrawString(font, "Game will restart in : " + (gameResetTimer / 100), new Vector2(600, 360), Color.Red);

                _spriteBatch.End();
                if (gameResetTimer < 1)
                    resetGame();
                else
                    gameResetTimer--;
            }
            base.Draw(gameTime);

        }
    }
}
