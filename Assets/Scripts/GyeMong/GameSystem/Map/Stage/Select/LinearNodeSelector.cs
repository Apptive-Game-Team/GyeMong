using System.Collections.Generic;
using GyeMong.GameSystem.Map.Stage.Select.Node;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.Select
{
    // This class is responsible for selecting the closed node in a linear fashion.
    public class LinearNodeSelector : INodeSelector<StageNode>
    {
        private List<StageNode> _stageNodes;
        private int _maxIndex;
        
        public LinearNodeSelector(List<StageNode> stageNodes, int maxIndex)
        {
            _maxIndex = maxIndex;
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

            direction.Normalize();
            
            if (prevIndex >= 0)
            {
                Vector3 toPrev =
                    (_stageNodes[prevIndex].transform.position - _stageNodes[currentIndex].transform.position)
                    .normalized;
                float dotPrev = Vector3.Dot(direction, toPrev);

                if (dotPrev > bestDot && dotPrev > 0f)
                {
                    bestDot = dotPrev;
                    bestNode = _stageNodes[prevIndex];
                }
            }
            
            if (nextIndex <=  Mathf.Min(_maxIndex, _stageNodes.Count - 1))
            {
                Vector3 toNext =
                    (_stageNodes[nextIndex].transform.position - _stageNodes[currentIndex].transform.position)
                    .normalized;
                float dotNext = Vector3.Dot(direction, toNext);

                if (dotNext > bestDot && dotNext > 0f)
                {
                    bestDot = dotNext;
                    bestNode = _stageNodes[nextIndex];
                }
            }
            Debug.Log("Selecting node in direction: " + direction);
            Debug.Log("Current node: " + currentNode.gameObject.name);
            Debug.Log("Best node: " + (bestNode != null ? bestNode.gameObject.name : "null"));
            return bestNode;
        }
    }
}