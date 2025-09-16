using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core;
using State = Survivor.Classes.Core.Enums.State;
using Survivor.Classes.Controllers;
using Survivor.Classes.Core.Enums;
namespace Survivor
{
    public partial class Game1 : Game
    {
        public Animator.DrawData LoadItemData()
        {
            return new Animator.DrawData(
                Content.Load<Texture2D>("GoldCoin"), 10, //idle
                Content.Load<Texture2D>("HeartCoin"), 10, //run
                Content.Load<Texture2D>("whiteRune"), 8, //attack
                Content.Load<Texture2D>("redRune"), 8, //dead
                Content.Load<Texture2D>("GoldCoin"), 10,
                new Vector2(13, 13),
                new Vector2(26, 26)
            );

        }
        public void LoadDropDataAndGenerateDrops(List<Vector2> dropLocations) =>
            _dropController.HandleDrops(dropLocations, LoadItemData());


        public Animator.DrawData LoadEffectData()
        {
            return new Animator.DrawData(
                Content.Load<Texture2D>("player_jump"), 9, //idle
                Content.Load<Texture2D>("player_fireball"), 14, //run
                Content.Load<Texture2D>("burn_enemies"), 8, //attack
                Content.Load<Texture2D>("burn_enemies"), 1, //dead
                Content.Load<Texture2D>("player_fireBall"), 14,
                new Vector2(50, 13),
                new Vector2(100, 50)
            );
        }

        public void LoadAndAddEffect(Vector2 position, State state) =>
            _effectController.AddEffect(LoadEffectData(), state, position, new(0, 0), new(0, 0));

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
                    LoadAndAddEffect(_player.Position.Position, State.Idle);
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

            if (attackTimer > 0)//Prevent second attack animation, before first finishes
            {
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.SetState(State.Attacking);
            }

            if (inputs.Contains(InputState.Special))
            {
                if (_fireBallController == null && _player.Mana >= 50)
                {
                    int speed;
                    if (_player.Direction == "right")
                        speed = 6;
                    else
                        speed = -6;
                    Vector2 FireBallSpawn = new(_player.Position.Position.X, _player.Position.Position.Y - 20);
                    _fireBallController = new FireBallController(LoadEffectData(), _player.Direction, new(220, 220), FireBallSpawn, speed);
                    _player.ReduceMana(50);
                    fireball.Play();
                }
            }

            else if (inputs.Contains(InputState.Attack))//handle attack input
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
                        damageZoneEnd = new(_player.Position.Position.X + 100, _player.Position.Position.Y + _player.Size.Size.Y / 2 + 30);
                    }
                    else
                    {
                        damageZoneStart = new(_player.Position.Position.X - 100, _player.Position.Position.Y - _player.Size.Size.Y / 2 + 30);
                        damageZoneEnd = new(_player.Position.Position.X, _player.Position.Position.Y + _player.Size.Size.Y / 2 + 30);
                    }

                    List<Vector2> DropSpawnLocations = _enemyController.KillEnemies(damageZoneStart, damageZoneEnd);
                    LoadDropDataAndGenerateDrops(DropSpawnLocations);
                }
            }
            _player.Walk(dx, dy);//move player
            _player.Position.SetPosition(_worldBoundsController.PushToWorldBounds(_player.Position.Position, _player.Size.Size));//push back in if out of bounds
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
                if (damageTaken > 0)
                    _invulnerabilityTimeLeft = _invulnerabilityTimer;
                _player.TakeDamage(damageTaken);
            }
        }

        public void ResetGame()//reset game world
        {
            _gameLevel = 1;
            gameResetTimer = 500;
            gamePaused = false;
            _player = null;
            ConstructPlayer();
            _enemyController.ResetEnemies();
            _enemyController.ResetSpawnedEnemies();
            FillEnemies();
        }

        public bool FireballStillActive()
        {
            if (_fireBallController == null)
                return false;
            else
                return (int)_fireBallController.Position.Position.X > _worldBounds.WorldStart.X &&
                        (int)_fireBallController.Position.Position.X < _worldBounds.WorldEnd.X;
        }
        
    }
}