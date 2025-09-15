using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core.Enums;
namespace Survivor.Classes.Core
{
    public class DroppedItem : GameObject
    {
        private State _type;
        private int _lifeTime;
        private int _effectLeft;
        private readonly Animator _Animator;
        public DroppedItem(Animator.DrawData drawData, State type, int time, Vector2 position) : base((int)position.X, (int)position.Y, 50, 50, 0, 0)
        {
            _type = type;
            _lifeTime = time;
            _effectLeft = 500;
            _Animator = new Animator(drawData, type);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
            _Animator.Draw(spriteBatch, Position.Position, "Item", gameTime);

        public override void Update(Vector2 playerPosition)
        {
            _lifeTime--;
            _effectLeft--;
        }

        public bool ItemStillAvailable => _lifeTime > 1;

        public bool ItemStillActive => _effectLeft > 1;

        public State ItemType => _type;
    }
}