using UnityEngine;

namespace System.Game.Portal
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] PortalID portalID;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            StartCoroutine(PortalManager.Instance.TransitScene(portalID));
        }
    }
}
