using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State = Survivor.Classes.Core.Enums.State;
namespace Survivor.Classes.Core
{
    public class SkillEffect : GameObject
    {
        private Animator _animator;
        private int _animationLength;
        public SkillEffect(Animator.DrawData drawData, State type, Vector2 position, Vector2 size, Vector2 speed)
            : base((int)position.X, (int)position.Y, (int)size.X, (int)size.Y, speed.X, speed.Y)
        {
            _animator = new Animator(drawData, type);
            if (type == State.Idle)
                _animationLength = drawData.idleFrames * 2;
            if (type == State.Running)
                _animationLength = drawData.runFrames * 2;
            if(type == State.Attacking)
                _animationLength = drawData.attackFrames * 2;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
            _animator.Draw(spriteBatch, Position.Position, "Item", gameTime);

        public override void Update(Vector2 playerPosition) =>
            _animationLength--;

        public bool AnimationFinished => _animationLength <= 0;
    }
}