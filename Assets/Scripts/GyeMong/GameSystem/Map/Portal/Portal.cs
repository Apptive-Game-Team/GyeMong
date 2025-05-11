using UnityEngine;

namespace GyeMong.GameSystem.Map.Portal
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
