using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.Select
{
    // This interface is responsible for selecting the node based on the direction of the input.
    public interface INodeSelector<T>
    {
        public T SelectNode(T currentNode, Vector3 direction); 
    }
}