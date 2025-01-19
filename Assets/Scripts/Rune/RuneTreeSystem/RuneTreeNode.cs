using System.Collections.Generic;
using UnityEngine;

namespace runeSystem.RuneTreeSystem
{
    public class RuneTreeNode : RuneUIObject, ITreeLayoutNode
    {
        [SerializeField] private RuneTreeNode _parent;
        [SerializeField] private List<RuneTreeNode> _children = new List<RuneTreeNode>();
        private int _depth;
        private Transform _transform;

        public static RuneTreeNode Create(RuneTreeNode parent, RuneData runeData)
        {
            RuneTreeNode newNode = new RuneTreeNode();
            newNode.Init(parent, runeData);
            return newNode;
        }
        
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
            _children.Add(runeTreeNode);
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