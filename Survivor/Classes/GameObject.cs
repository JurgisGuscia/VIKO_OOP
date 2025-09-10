using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoveVector = System.Numerics.Vector2;
using CoordVector = System.Numerics.Vector2;
namespace Survivor.Classes
{
    public abstract class GameObject
    {
        public Position Position { get; private set; }
        public Size Size { get; private set; }
        public WorldBounds WorldBounds { get; private set; }
        public ObjectVelocity Velocity { get; private set; }

        public GameObject(WorldBounds bounds, int x = 0, int y = 0, int width = 100, int height = 200, float speedX = 0, float speedY = 0)
        {
            WorldBounds = bounds;
            Position = new Position(x, y);
            Size = new Size(width, height);
            Velocity = new ObjectVelocity(speedX, speedY);
        }

        public virtual void Walk(int dx, int dy)
        {
            MoveVector move = new(dx, dy);
            Position.Move(move);
        }

        public virtual void Move(int dx, int dy)
        {
            MoveVector move = new(dx, dy);
            Velocity.AddVelocity(move);
            Velocity.ApplyVelocity();
            Position.Move(Velocity.Speed);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public abstract void Update(CoordVector playerPosition);
    }
}