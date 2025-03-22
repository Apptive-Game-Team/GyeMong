using System.Collections.Generic;
using System.Game.Rune.RuneUI.RuneTreeSystem.TreeLayout;
using UnityEngine;

namespace System.Game.Rune.RuneUI.RuneTreeSystem
{
    public class RuneTreeNode : RuneUIObject, ITreeLayoutNode
    {
        [SerializeField] private RuneTreeNode _parent;
        [SerializeField] private List<RuneTreeNode> _children = new List<RuneTreeNode>();
        private int _depth;
        private Transform _transform;
        
        public void Init(RuneTreeNode parent, RuneData newData)
        {
            _parent = parent;
            if (_parent != null)
            {
                _parent.SetChild(this);
                _depth = _parent.GetDepth() + 1;
            }
            else
            {
                _depth = 0;
            }
            Init(newData);
        }

        private void SetChild(RuneTreeNode runeTreeNode)
        {
            if (runeTreeNode == this)
            {
                Debug.LogError("Cannot set a node as its own child.");
                return;
            }

            if (!_children.Contains(runeTreeNode))
            {
                _children.Add(runeTreeNode);
            }
        }

        Transform ITreeLayoutNode.transform => transform;

        public int GetDepth()
        {
            return _depth;
        }

        public int GetChildrenCount()
        {
            return _children.Count;
        }

        public ITreeLayoutNode GetChild(int index)
        {
            return _children[index];
        }

        public ITreeLayoutNode GetParent()
        {
            return _parent;
        }
    }
}