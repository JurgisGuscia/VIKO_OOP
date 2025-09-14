using Survivor.Classes.Core;
using Survivor.Classes.Core.Utils;
using Survivor.Classes.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Survivor.Classes.Core.Enums;
namespace Survivor.Classes.Controllers
{
    public class EnemyController
    {
        private Enemy[] _enemies;
        private IWorldBounds _bounds;
        private int _maxEnemies;
        private int _enemiesSpawned = 0;
        private int _enemiesPerCycle;

        public EnemyController(int enemyCount, IWorldBounds bounds, int enemiesPerCycle)
        {
            _maxEnemies = enemyCount;
            _enemies = new Enemy[enemyCount];
            _bounds = bounds;
            _enemiesPerCycle = enemiesPerCycle;
        }

        public int SpawnedEnemies => _enemiesSpawned;

        public void ResetSpawnedEnemies() => _enemiesSpawned = 0;
        public void ResetEnemies() => _enemies = new Enemy[_maxEnemies];

        public bool AllEnemiesDead()
        {
            foreach (Enemy enemy in _enemies)
                if (enemy != null)
                    return false;
            return true;
        }

        public void FillEnemies(int enemyLimit, Animator.DrawData drawData, Vector2 size)
        {
            for (int i = 0; i < _enemiesPerCycle; i++)
                if (_enemiesSpawned < enemyLimit)
                    for (int j = 0; j < _maxEnemies; j++)
                        if (_enemies[j] == null)
                        {
                            _enemies[j] = new Enemy(_bounds, drawData, size, Util.PickSpawnLocation(_bounds));
                            _enemiesSpawned++;
                            break;
                        }
        }

        public void DrawEnemies(IWorldBoundsController worldController, SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Enemy enemy in _enemies)
                if (enemy != null)
                {
                    worldController.PushToWorldBounds(enemy.Position.Position, enemy.Size.Size);
                    enemy.Draw(spriteBatch, gameTime);
                }
        }

        public void UpdateEnemies(IWorldBoundsController worldController, int gameLevel, Player player)
        {
            for (int i = 0; i < _maxEnemies; i++)
                if (_enemies[i] != null)
                {
                    if (_enemies[i].Position.Position.Y < _bounds.WorldEnd.Y - _enemies[i].Size.Size.Y / 2)
                        _enemies[i].Velocity.ApplyForce(new(0, 0.5f));
                    else
                        _enemies[i].Velocity.ResetAccelerationY();

                    if (_enemies[i].State == State.Dead && _enemies[i].AnimationFinished())
                    {
                        _enemies[i] = null;
                        player.AddScore(gameLevel * 10);
                    }
                    else
                    {
                        _enemies[i].Update(player.Position.Position);
                        _enemies[i].Position.SetPosition(worldController.PushToWorldBounds(_enemies[i].Position.Position, _enemies[i].Size.Size));
                    }
                }
        }

        public void KillEnemies(Vector2 damageZoneStart, Vector2 damageZoneEnd)
        {
            for (int i = 0; i < _maxEnemies; i++)
                if (_enemies[i] != null)
                {
                    int XOffset = Util.ToInt(_enemies[i].Size.Size.X / 2);
                    int YOffset = Util.ToInt(_enemies[i].Size.Size.Y / 2);
                    var checkPoints = new Vector2[4]
                    {
                        new Vector2(_enemies[i].Position.Position.X - XOffset, _enemies[i].Position.Position.Y - YOffset),
                        new Vector2(_enemies[i].Position.Position.X + XOffset, _enemies[i].Position.Position.Y - YOffset),
                        new Vector2(_enemies[i].Position.Position.X - XOffset, _enemies[i].Position.Position.Y + YOffset),
                        new Vector2(_enemies[i].Position.Position.X + XOffset, _enemies[i].Position.Position.Y + YOffset)
                    };

                    for (int j = 0; j < 4; j++)
                        if (checkPoints[j].X > damageZoneStart.X && checkPoints[j].X < damageZoneEnd.X &&
                            checkPoints[j].Y > damageZoneStart.Y && checkPoints[j].Y < damageZoneEnd.Y)
                        {
                            _enemies[i].SetState(State.Dead);
                            break;
                        }
                }
        }

        public bool Collided(Enemy enemy, Vector2[] playerBox)
        {
            if (enemy == null)
                return false;
            else
                foreach (Vector2 corner in playerBox)
                    if (corner.X > enemy.Size.StartPoint(enemy.Position.Position).X &&
                        corner.X < enemy.Size.EndPoint(enemy.Position.Position).X &&
                        corner.Y > enemy.Size.StartPoint(enemy.Position.Position).Y &&
                        corner.Y < enemy.Size.EndPoint(enemy.Position.Position).Y
                    )
                        return true;
            return false;
        }

        public int CalculateCollisions(Player player)
        {
            int damageSuffered = 0;
            int XOffset = (int)player.Size.Size.X / 2;
            int YOffset = (int)player.Size.Size.Y / 2;
            var playerBox = new[]
            {
                new Vector2(player.Position.Position.X - XOffset, player.Position.Position.Y - YOffset),
                new Vector2(player.Position.Position.X + XOffset, player.Position.Position.Y - YOffset),
                new Vector2(player.Position.Position.X - XOffset, player.Position.Position.Y + YOffset),
                new Vector2(player.Position.Position.X + XOffset, player.Position.Position.Y + YOffset)
            };
            foreach (Enemy enemy in _enemies)
                if (Collided(enemy, playerBox))
                    damageSuffered++;
            return damageSuffered;
        }

        public bool ShouldLevelUp(int gameLevel) => _enemiesSpawned >= gameLevel * 10 && AllEnemiesDead();
   
    }
}