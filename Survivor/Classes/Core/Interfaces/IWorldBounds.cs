
namespace Survivor.Classes.Core.Interfaces
{
    public interface IWorldBounds
    {
        Vector2 WorldSize { get; }
        Vector2 WorldStart { get; }
        Vector2 WorldEnd { get; }
        void SetWorldSize(Vector2 newSize);
    }
}
