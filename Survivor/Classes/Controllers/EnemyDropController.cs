using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core;
using Survivor.Classes.Core.Enums;
namespace Survivor.Classes.Controllers
{
    public class EnemyDropController
    {
        private int _totalDropChance = 60;
        private int _itemLifeTime = 200;
        private int _itemScore = 0;
        private int _itemHeal = 0;
        private List<DroppedItem> _activeDrops = new List<DroppedItem>();
        Random random = new Random();
        dynamic[] _dropTable = new[]
        {
            new{
                Group = "Normal drops",
                Chance = 70,
                Items = new[]{
                    new { Item = State.Idle, Chance = 80 },
                    new { Item = State.Running, Chance = 20 }
                },
            },
            new{
                Group = "Special drops",
                Chance = 10,
                Items = new[]{
                    new { Item = State.Attacking, Chance = 50 },
                    new { Item = State.Dead, Chance = 50 }
                },
            }
        };
        public int ItemScore => _itemScore;

        public bool ShouldDrop() => random.Next(1, 101) <= _totalDropChance;

        public void GenerateDrops(List<Vector2> dropLocations, Animator.DrawData drawData)
        {
            foreach (Vector2 location in dropLocations)
            {
                foreach (var group in _dropTable)
                {
                    int dropChecker = random.Next(1, 101);
                    if (dropChecker <= group.Chance)
                    {
                        int chanceSum = 0;
                        int subDropChecker = random.Next(1, 101);
                        foreach (var item in group.Items)
                        {
                            chanceSum += item.Chance;
                            if (subDropChecker <= chanceSum)
                            {
                                _activeDrops.Add(new DroppedItem(drawData, item.Item, _itemLifeTime, location));
                                break;
                            }
                        }
                    }
                }
            }

        }

        public void HandleDrops(List<Vector2> dropLocations, Animator.DrawData drawData) 
        {
            if (ShouldDrop())
                GenerateDrops(dropLocations, drawData);
        }

        public void DrawDrops(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (DroppedItem item in _activeDrops)
            {
                item.Draw(spriteBatch, gameTime);
            }
        }

        public void Update(Vector2 playerPosition)
        {
            for (int i = _activeDrops.Count - 1; i >= 0; i--)
            {
                _activeDrops[i].Update(playerPosition);
                if (!_activeDrops[i].ItemStillAvailable)
                    _activeDrops.RemoveAt(i);
            }
        }

        public void HandlePickedUpItems(Vector2 playerPosition, WorldController worldController)
        {
            for (int i = _activeDrops.Count - 1; i >= 0; i--)
            {
                float distanceX = playerPosition.X - _activeDrops[i].Position.Position.X;
                float distanceY = playerPosition.Y - _activeDrops[i].Position.Position.Y;
                if (distanceX > -25 && distanceX < 25 && distanceY > -50 && distanceY < 50)
                {
                    if (_activeDrops[i].ItemType == State.Idle)//handle coin
                        _itemScore += 50;
                    if (_activeDrops[i].ItemType == State.Running)//handle heart
                        _itemHeal += 5;
                    if (_activeDrops[i].ItemType == State.Dead)//handle red rune
                        worldController.ActiveBurnEnemies();
                    if (_activeDrops[i].ItemType == State.Attacking)//handle white rune
                        worldController.ActivePushFromPlayer();
                        
                    _activeDrops.RemoveAt(i);
                }
            }
        }

        public int GetItemScore()
        {
            int score = _itemScore;
            _itemScore = 0;
            return score;
        }

        public int GetItemHeal()
        {
            int heal = _itemHeal;
            _itemHeal = 0;
            return heal;
        }

    }
}