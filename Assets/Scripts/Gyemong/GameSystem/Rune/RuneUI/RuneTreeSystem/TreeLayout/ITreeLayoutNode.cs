using UnityEngine;

namespace Gyemong.GameSystem.Rune.RuneUI.RuneTreeSystem.TreeLayout
{
    public interface ITreeLayoutNode
    {
        Transform transform { get;}
        int GetDepth();
        int GetChildrenCount();
        ITreeLayoutNode GetChild(int index);
        ITreeLayoutNode GetParent();
    }
}