using System.Collections.Generic;
using UnityEngine;

namespace runeSystem.RuneTreeSystem
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