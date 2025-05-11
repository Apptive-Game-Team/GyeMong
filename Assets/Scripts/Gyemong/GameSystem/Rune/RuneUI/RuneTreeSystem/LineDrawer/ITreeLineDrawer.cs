using UnityEngine;

namespace Gyemong.GameSystem.Rune.RuneUI.RuneTreeSystem.LineDrawer
{
    public interface ITreeLineDrawer
    {
        void ClearLines();
        void ConnectNodes(Vector2 position, Vector2 vector2);
    }
}