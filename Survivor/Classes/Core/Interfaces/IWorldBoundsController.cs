
namespace Survivor.Classes.Core.Interfaces
{
    public interface IWorldBoundsController
    {
        Vector2 PushToWorldBounds(Vector2 position, Vector2 size);
    }
}