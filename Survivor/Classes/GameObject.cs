using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoveVector = System.Numerics.Vector2;

namespace Survivor.Classes
{
    public abstract class GameObject
    {
        public Position Position { get; private set; }
        public Size Size { get; private set; }
        public WorldBounds WorldBounds { get; private set; }

        public GameObject(WorldBounds bounds, int x = 0, int y = 0, int width = 100, int height=200)
        {
            WorldBounds = bounds;
            Position = new Position(x, y);
            Size = new Size(width, height);
        }

        public virtual void Move(int dx, int dy)
        {
            MoveVector move = new(dx, dy);
            Position.Move(move);
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}