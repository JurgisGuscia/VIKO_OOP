using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core.Components;
namespace Survivor.Classes.Core
{
    public abstract class GameObject
    {
        public ObjectPosition Position { get; }
        public ObjectSize Size { get; set; }
        public ObjectVelocity Velocity { get; private set; }

        public GameObject(int x, int y, int width, int height, float speedX = 0, float speedY = 0)
        {
            Position = new ObjectPosition(x, y);
            Size = new ObjectSize(width, height);
            Velocity = new ObjectVelocity(speedX, speedY);
        }

        public virtual void Walk(int dx, int dy)
        {
            Vector2 move = new(dx, dy);
            Position.Move(move);
        }

        public virtual void Move(int dx, int dy)
        {
            Vector2 move = new(dx, dy);
            Velocity.ApplyForce(move);
            Velocity.AddVelocity();
            Position.Move(Velocity.Velocity);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public abstract void Update(Vector2 playerPosition);
    }
}