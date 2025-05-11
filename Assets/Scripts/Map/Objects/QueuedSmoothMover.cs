using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects
{
    public class QueuedSmoothMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        
        private Queue<Vector3> _moveQueue = new Queue<Vector3>();
        private Coroutine _moveCoroutine;

        private IEnumerator MoveOnQueue()
        {
            while (_moveQueue.Count > 0)
            {
                Vector3 destination = _moveQueue.Dequeue();
                while (Vector3.Distance(transform.position, destination) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
            _moveCoroutine = null;
        }
        
        public void MoveTo(Vector3 destination)
        {
            _moveQueue.Enqueue(destination);
            if (_moveCoroutine == null)
            {
                _moveCoroutine = StartCoroutine(MoveOnQueue());
            }
        }
    }
}
