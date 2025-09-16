using System.Numerics;
namespace Survivor.Classes.Core.Components
{
    public class ObjectVelocity(float x, float y)
    {
        private Vector2 _velocity = new(x, y);
        private Vector2 _acceleration = Vector2.Zero;

        public Vector2 Velocity => _velocity;
        public Vector2 Acceleration => _acceleration;

        public void ApplyForce(Vector2 force) => _acceleration += force;
        public void AddVelocity()
        {
            _velocity += _acceleration;
            _acceleration = Vector2.Zero;
        }

        public void SetAccelerationX(float x) => _acceleration.X = x;
        public void SetAccelerationY(float x) => _acceleration.Y = y;
        public void ResetAcceleration() => _acceleration = Vector2.Zero;
        public void ResetAccelerationX() => _acceleration.X = 0f;
        public void ResetAccelerationY() => _acceleration.Y = 0f;

        public void SetVelocity(Vector2 velocity) => _velocity = velocity;
        public void SetVelocityX(float x) => _velocity.X = x;
        public void SetVelocityY(float y) => _velocity.Y = y;
        public void ResetVelocity() => _velocity = Vector2.Zero;
        public void ResetVelocityX() => _velocity.X = 0f;
        public void ResetVelocityY() => _velocity.Y = 0f;
    }
}
