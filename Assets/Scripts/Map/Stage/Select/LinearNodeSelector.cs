using System.Collections.Generic;

using Map.Stage.Select.Node;
using UnityEngine;

namespace Map.Stage.Select
{
    // This class is responsible for selecting the closed node in a linear fashion.
    public class LinearNodeSelector : INodeSelector<StageNode>
    {
        private List<StageNode> _stageNodes;
        
        public LinearNodeSelector(List<StageNode> stageNodes)
        {
            _stageNodes = stageNodes;
        }
        

        public StageNode SelectNode(StageNode currentNode, Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return null;
            }
            int currentIndex = _stageNodes.IndexOf(currentNode);
            int prevIndex = currentIndex - 1;
            int nextIndex = currentIndex + 1;

            StageNode bestNode = null;
            float bestDot = -Mathf.Infinity;

            if (prevIndex >= 0)
            {
                Vector3 toPrev = (_stageNodes[prevIndex].transform.position - _stageNodes[currentIndex].transform.position).normalized;
                float dotPrev = Vector3.Dot(direction.normalized, toPrev);

                if (dotPrev > bestDot)
                {
                    bestDot = dotPrev;
                    bestNode = _stageNodes[prevIndex];
                }
            }

            if (nextIndex < _stageNodes.Count)
            {
                Vector3 toNext = (_stageNodes[nextIndex].transform.position - _stageNodes[currentIndex].transform.position).normalized;
                float dotNext = Vector3.Dot(direction.normalized, toNext);

                if (dotNext > bestDot)
                {
                    bestDot = dotNext;
                    bestNode = _stageNodes[nextIndex];
                }
            }

            return bestNode;
        }
        
    }
}