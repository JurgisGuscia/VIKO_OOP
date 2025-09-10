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
        Song backgroundSong;
        Texture2D spriteSheetIdle;
        Texture2D spriteSheetRun;
        Texture2D spriteSheetAttack;
        Texture2D spriteSheetDead;
        Texture2D backgroundTexture;
        Texture2D landTexture;
        private Texture2D _pixel;
        private float attackTimer = 0f;
        SoundEffectInstance run;
        SoundEffectInstance jump;
        SoundEffectInstance land;
        SoundEffectInstance swing;
        SoundEffectInstance slash;
        public int animationTime = 0;

        public bool gamePaused = false;
        public int gameResetTimer = 500;
        private int gameLevel = 1;
        private int invulnerabilityTimer = 24;
        private int invulnerabilityTimeLeft = 0;
        private SpriteFont font;
        private WorldBounds worldBounds;
        private Player _player;
        private static int _enemyCount = 100;
        public Enemy[] enemies = new Enemy[_enemyCount];
        
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
        public Vector2 damageZoneStart;
        public Vector2 damageZoneEnd;

        private void HandlePlayerInput(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            int dx = 0;
            int dy = 0;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (_player.State != PlayerState.Jumping)
                {
                    jump.Play();
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


            if (attackTimer > 0)
            {
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.State = PlayerState.Attacking;
            }
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
                        damageZoneStart = new(_player.Position.Coords.X -90, _player.Position.Coords.Y - 100);
                        damageZoneEnd = new(_player.Position.Coords.X , _player.Position.Coords.Y + 100);
                    }

                    for(int i = 0;  i < _enemyCount; i++)
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



        public void FillEnemies(int enemyCount) {
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemies[i] == null)
                {
                    int XPosition = random.Next(40, (int)Math.Round(worldBounds.WorldEndingBounds.X));
                    int YPosition = random.Next(40, (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 200);
                    enemies[i] = new Enemy(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: XPosition, y: YPosition, 30, 70);
                }
            }    
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            backgroundTexture = Content.Load<Texture2D>("Summer3");

            spriteSheetIdle = Content.Load<Texture2D>("IDLE");
            spriteSheetRun = Content.Load<Texture2D>("RUN");
            spriteSheetAttack = Content.Load<Texture2D>("attack");
            spriteSheetDead = Content.Load<Texture2D>("IDLE");


            landTexture = Content.Load<Texture2D>("4");
            font = Content.Load<SpriteFont>("DebugFont"); // A tiny default font
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: 600, y: (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 55, 28, 67);


            spriteSheetIdle = Content.Load<Texture2D>("zombie_idle");
            spriteSheetRun = Content.Load<Texture2D>("zombie_walk");
            spriteSheetAttack = Content.Load<Texture2D>("zombie_attack");
            spriteSheetDead = Content.Load<Texture2D>("zombie_death");


            FillEnemies(gameLevel * 10);

            

            backgroundSong = Content.Load<Song>("Mission");
            MediaPlayer.IsRepeating = true;   // loop automatically
            MediaPlayer.Volume = 0.1f;        // 0..1
            MediaPlayer.Play(backgroundSong);

            SoundEffect ambienceEffect = Content.Load<SoundEffect>("step");
            run = ambienceEffect.CreateInstance();
            run.IsLooped = true;
            run.Volume = 0.2f;
            run.Pitch = -0.2f;

            ambienceEffect = Content.Load<SoundEffect>("jump");
            jump = ambienceEffect.CreateInstance();
            jump.IsLooped = false;
            jump.Volume = 0.2f;
            jump.Pitch = 0f;

            ambienceEffect = Content.Load<SoundEffect>("land");
            land = ambienceEffect.CreateInstance();
            land.IsLooped = false;
            land.Volume = 0.2f;
            land.Pitch = 0f;

            ambienceEffect = Content.Load<SoundEffect>("swing");
            swing = ambienceEffect.CreateInstance();
            swing.IsLooped = false;
            swing.Volume = 0.2f;
            swing.Pitch = 0f;

            ambienceEffect = Content.Load<SoundEffect>("slash");
            slash = ambienceEffect.CreateInstance();
            slash.IsLooped = false;
            slash.Volume = 0.2f;
            slash.Pitch = 0f;

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void CheckLevelUp()
        {
            bool levelCompleted = true;
            for (int i = 0; i < _enemyCount; i++)
            {
                if (enemies[i] != null)
                    levelCompleted = false;
            }
            if (levelCompleted)
            {
                gameLevel++;
                if (gameLevel + 10 > 100)
                {
                    FillEnemies(100);
                }
                else
                {
                    FillEnemies(gameLevel + 10);
                }
                
            }
        }

        public bool CheckCollision(Vector2[] playerBodyCorners, Vector2 enemyBodyStart, Vector2 enemyBodyEnd) {
            //0-topleft, 1-topright, 2-bottomleft, 3-bottomright
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

                Vector2[] playerBodyCorners =
                [
                    playerTopLeft,
                playerTopRight,
                playerBottomLeft,
                PlayerBottomRight
                ];

                for (int i = 0; i < _enemyCount; i++)
                {
                    if (enemies[i] != null)
                    {
                        Vector2 enemyBodyStart = new(enemies[i].Position.Coords.X - enemies[i].Size.ObjectSize.X / 2, enemies[i].Position.Coords.Y - enemies[i].Size.ObjectSize.Y / 2);
                        Vector2 enemyBodyEnd = new(enemies[i].Position.Coords.X + enemies[i].Size.ObjectSize.X / 2, enemies[i].Position.Coords.Y + enemies[i].Size.ObjectSize.Y / 2);

                        if (CheckCollision(playerBodyCorners, enemyBodyStart, enemyBodyEnd))
                            damageTaken++;
                    }
                }
                if (damageTaken > 0)
                    invulnerabilityTimeLeft = invulnerabilityTimer;
                _player.TakeDamage(damageTaken);
            }
        }

        public void resetGame()
        {
            _player = null;
            spriteSheetIdle = Content.Load<Texture2D>("IDLE");
            spriteSheetRun = Content.Load<Texture2D>("RUN");
            spriteSheetAttack = Content.Load<Texture2D>("attack");
            spriteSheetDead = Content.Load<Texture2D>("IDLE");
            _player = new Player(worldBounds, spriteSheetIdle, spriteSheetRun, spriteSheetAttack, spriteSheetDead, x: 600, y: (int)Math.Round(worldBounds.WorldEndingBounds.Y) - 55, 28, 67);
            for (int i = 0; i < _enemyCount; i++)
            {
                enemies[i] = null;
            }
            gameLevel = 1;
            spriteSheetIdle = Content.Load<Texture2D>("zombie_idle");
            spriteSheetRun = Content.Load<Texture2D>("zombie_walk");
            spriteSheetAttack = Content.Load<Texture2D>("zombie_attack");
            spriteSheetDead = Content.Load<Texture2D>("zombie_death");
            FillEnemies(gameLevel * 10);
            gameResetTimer = 500;
            gamePaused = false;
        }


        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (!gamePaused)
            {
                // TODO: Add your update logic here

                if (_player.Position.Coords.Y < worldBounds.WorldEndingBounds.Y - 51)
                {
                    _player.Velocity.AddVelocity(new(0, 0.5f));
                }
                else
                {
                    _player.Velocity.ResetSpeedY();
                }

                _player.Update(_player.Position.Coords);

                for (int i = 0; i < _enemyCount; i++)
                {
                    if (enemies[i] != null)
                    {
                        if (enemies[i].Position.Coords.Y < worldBounds.WorldEndingBounds.Y - 51)
                        {
                            enemies[i].Velocity.AddVelocity(new(0, 0.5f));
                        }
                        else
                        {
                            enemies[i].Velocity.ResetSpeedY();
                        }
                        if (enemies[i].State == EnemyState.Dead)
                        {
                            if (enemies[i].DeathFrames == 0)
                            {
                                enemies[i] = null;
                                _player.AddScore(gameLevel * 10);
                            }
                            else
                            {
                                enemies[i].DeathFrames--;
                            }
                        }
                        else
                        {
                            enemies[i].Update(_player.Position.Coords);
                        }
                    }
                }
                CheckDamageTaken();
                CheckLevelUp();
                if (_player.Health < 1)
                    gamePaused = true;

                invulnerabilityTimeLeft--;
            }
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (!gamePaused)
            {
                HandlePlayerInput(gameTime);
                if (_player.State == PlayerState.Running)
                {
                    run.Play();
                }
                else
                {
                    run.Pause();
                }

                if (_player.State == PlayerState.Jumping && _player.Position.Coords.Y > worldBounds.WorldEndingBounds.Y - 51 && _player.Velocity.Speed.Y > 0)
                    land.Play();
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

                _spriteBatch.DrawString(font, "Coordinates " + _player.Position.Coords.X + " " + _player.Position.Coords.Y, new Vector2(10, 300), Color.White);

                PlayerVector playerBoxStart = _player.Size.StartPoint(_player.Position.Coords);

                _spriteBatch.DrawString(font, "Speed X " + (int)Math.Round(_player.Velocity.Speed.X), new Vector2(10, 330), Color.White);
                _spriteBatch.DrawString(font, "Speed Y " + (int)Math.Round(_player.Velocity.Speed.Y), new Vector2(10, 350), Color.White);
                _spriteBatch.DrawString(font, "Velocity X " + (int)Math.Round(_player.Velocity.Velocity.X), new Vector2(10, 370), Color.White);
                _spriteBatch.DrawString(font, "Velocity Y " + (int)Math.Round(_player.Velocity.Velocity.Y), new Vector2(10, 390), Color.White);

                _spriteBatch.DrawString(font, "State " + _player.State, new Vector2(10, 410), Color.White);

                _spriteBatch.DrawString(font, "HitBox start X" + (int)Math.Round(damageZoneStart.X), new Vector2(10, 430), Color.White);
                _spriteBatch.DrawString(font, "HitBox start Y" + (int)Math.Round(damageZoneStart.Y), new Vector2(10, 450), Color.White);
                _spriteBatch.DrawString(font, "HitBox end X" + (int)Math.Round(damageZoneEnd.X), new Vector2(10, 470), Color.White);
                _spriteBatch.DrawString(font, "HitBox end Y" + (int)Math.Round(damageZoneEnd.Y), new Vector2(10, 490), Color.White);

                //Draw health
                _spriteBatch.Draw(_pixel, new Rectangle(400, 50, _player.Health * 5, 20), Color.Red);
                _spriteBatch.DrawString(font, "HP:" + _player.Health, new Vector2(410, 52), Color.White);
                //Draw level and score
                _spriteBatch.DrawString(font, "Level: " + gameLevel, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(10, 30), Color.White);


                //_spriteBatch.Draw(_pixel, new Rectangle((int)Math.Round(playerBoxStart.X), (int)Math.Round(playerBoxStart.Y), (int)Math.Round(_player.Size.ObjectSize.X), (int)Math.Round(_player.Size.ObjectSize.Y)), Color.Green);



                _player.Draw(_spriteBatch, gameTime);

                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null)
                    {
                        enemy.HandleOutOfBounds();
                        // _spriteBatch.Draw(_pixel, new Rectangle(
                        //     (int)Math.Round(enemy.Position.Coords.X - (int)Math.Round(enemy.Size.ObjectSize.X) / 2),
                        //     (int)Math.Round(enemy.Position.Coords.Y - (int)Math.Round(enemy.Size.ObjectSize.Y) / 2),
                        //     (int)Math.Round(enemy.Size.ObjectSize.X),
                        //     (int)Math.Round(enemy.Size.ObjectSize.Y)),
                        //     Color.Red);

                        enemy.Draw(_spriteBatch, gameTime);
                    }
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
                DrawGround(0);
                DrawGround(500);
                DrawGround(1000);

                _spriteBatch.DrawString(font, "Coordinates " + _player.Position.Coords.X + " " + _player.Position.Coords.Y, new Vector2(10, 300), Color.White);

                PlayerVector playerBoxStart = _player.Size.StartPoint(_player.Position.Coords);

                _spriteBatch.DrawString(font, "Speed X " + (int)Math.Round(_player.Velocity.Speed.X), new Vector2(10, 330), Color.White);
                _spriteBatch.DrawString(font, "Speed Y " + (int)Math.Round(_player.Velocity.Speed.Y), new Vector2(10, 350), Color.White);
                _spriteBatch.DrawString(font, "Velocity X " + (int)Math.Round(_player.Velocity.Velocity.X), new Vector2(10, 370), Color.White);
                _spriteBatch.DrawString(font, "Velocity Y " + (int)Math.Round(_player.Velocity.Velocity.Y), new Vector2(10, 390), Color.White);

                _spriteBatch.DrawString(font, "State " + _player.State, new Vector2(10, 410), Color.White);

                _spriteBatch.DrawString(font, "HitBox start X" + (int)Math.Round(damageZoneStart.X), new Vector2(10, 430), Color.White);
                _spriteBatch.DrawString(font, "HitBox start Y" + (int)Math.Round(damageZoneStart.Y), new Vector2(10, 450), Color.White);
                _spriteBatch.DrawString(font, "HitBox end X" + (int)Math.Round(damageZoneEnd.X), new Vector2(10, 470), Color.White);
                _spriteBatch.DrawString(font, "HitBox end Y" + (int)Math.Round(damageZoneEnd.Y), new Vector2(10, 490), Color.White);

                //Draw health
                _spriteBatch.Draw(_pixel, new Rectangle(400, 50, _player.Health * 5, 20), Color.Red);
                _spriteBatch.DrawString(font, "HP:" + _player.Health, new Vector2(410, 52), Color.White);
                //Draw level and score
                _spriteBatch.DrawString(font, "Level: " + gameLevel, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(10, 30), Color.White);


                _spriteBatch.DrawString(font, "You lost", new Vector2(600, 300), Color.Red);
                _spriteBatch.DrawString(font, "Score: " + _player.Score, new Vector2(600, 320), Color.Red);
                _spriteBatch.DrawString(font, "Level reached: " + gameLevel, new Vector2(600, 340), Color.Red);
                _spriteBatch.DrawString(font, "Game will restart in : " + (gameResetTimer/100), new Vector2(600, 360), Color.Red);

                _spriteBatch.End();
                if (gameResetTimer < 1)
                {
                    resetGame();
                }
                else
                {
                    gameResetTimer--;
                }
            }
            base.Draw(gameTime);
             
        }
    }
}
