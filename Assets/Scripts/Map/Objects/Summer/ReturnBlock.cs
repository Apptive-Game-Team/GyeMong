using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Objects.Summer
{
    public class ReturnBlock : MonoBehaviour
    {
        [SerializeField] private Transform teleportTarget;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && teleportTarget != null)
            {
                Debug.Log("Hit");

                Rigidbody2D rb = collision.attachedRigidbody;
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.position = teleportTarget.position;
                }
                else
                {
                    collision.transform.position = teleportTarget.position;
                }
            }
        }
    }
}
