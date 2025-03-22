using UnityEngine;

namespace runeSystem.RuneTreeSystem
{
    public interface ITreeLineDrawer
    {
        void ClearLines();
        void ConnectNodes(Vector2 position, Vector2 vector2);
    }
}