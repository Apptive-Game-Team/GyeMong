using UnityEngine;

namespace GyeMong.GameSystem.Creature.Direction
{
    // Gizmo to visualize the direction controlled by DirectionController
    public class DirectionVisualizer : MonoBehaviour
    {
        private DirectionController _controller;

        private void Awake()
        {
            _controller = GetComponent<DirectionController>();
            if (_controller == null)
            {
                Debug.LogError("DirectionVisualizer requires a DirectionController component on the same GameObject.");
                Destroy(this);
            }
        }
        
        private void Update()
        {
            Vector3 dir = _controller.GetDirection();
            Debug.DrawRay(transform.position, dir * 5, Color.red);
        }
    }
}