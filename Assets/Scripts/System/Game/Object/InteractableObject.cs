using System.Input;
using UnityEngine;

namespace System.Game.Object
{
    public abstract class InteractableObject : MonoBehaviour
    {
        protected abstract void OnInteraction(Collider2D collision);

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && InputManager.Instance.GetKeyDown(ActionCode.Interaction))
                OnInteraction(collision);
        }
    }
}
