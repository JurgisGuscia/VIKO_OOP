using System;
using Survivor.Classes.Core.Interfaces;
namespace Survivor.Classes.Controllers
{
    public class WorldBoundsController : IWorldBoundsController
    {
        private readonly IWorldBounds _worldBounds;

        public WorldBoundsController(IWorldBounds worldBounds) => _worldBounds = worldBounds;

        public Vector2 PushToWorldBounds(Vector2 position, Vector2 size)
        {
            Vector2 WStart = _worldBounds.WorldStart;
            Vector2 WEnd = _worldBounds.WorldEnd;
            float offsetX = size.X / 2;
            float offsetY = size.Y / 2;
            int x = (int)Math.Clamp(position.X, WStart.X + offsetX, WEnd.X - offsetX);
            int y = (int)Math.Clamp(position.Y, WStart.Y + offsetY, WEnd.Y - offsetY - 20);
            return new Vector2(x, y);
        }
    }
}
