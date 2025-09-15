using Survivor.Classes.Core;
using Survivor.Classes.Core.Interfaces;
using Survivor.Classes.Core.Enums;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Survivor.Classes.Controllers
{
    public class FireBallController : GameObject
    {
        private bool _fireBallActive;
        private string _direction;
        private int _speed;
        Animator _animator;
        public FireBallController(Animator.DrawData drawData, string direction, Vector2 boxSize, Vector2 position, int speed)
            : base((int)position.X, (int)position.Y, (int)boxSize.X, (int)boxSize.Y)
        {
            _fireBallActive = true;
            _direction = direction;
            _speed = speed;
            _animator = new Animator(drawData, State.Jumping);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_fireBallActive)
                _animator.Draw(spriteBatch, Position.Position, _direction, gameTime);
        }

        public override void Update(Vector2 playerPosition)
        {
            if (_fireBallActive)
                Walk(_speed, 0);
        }
    }
}