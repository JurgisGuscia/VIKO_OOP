using System.Numerics;

namespace Survivor.Classes
{
    public class ObjectVelocity(float x, float y)
    {
        private Vector2 _speed = new(x, y);
        private Vector2 _velocity = Vector2.Zero;

        public Vector2 Velocity => _velocity;
        public Vector2 Speed => _speed;

        public void AddVelocity(Vector2 force) => _velocity += force;
        public void ApplyVelocity() => _speed += _velocity;

        public void ResetVelocity() => _velocity *= Vector2.Zero;
        public void SetSpeedX(float speed) => _speed.X = speed;
        public void ResetSpeed() => _speed *= Vector2.Zero;
        public void ResetSpeedX() => _speed.X *= 0f;
        public void ResetSpeedY() => _speed.Y *= 0f;
    }
}