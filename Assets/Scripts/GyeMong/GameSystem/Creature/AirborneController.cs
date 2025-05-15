using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature
{
    public class AirborneController : MonoBehaviour
    {
        [SerializeField] private GameObject shadow;
        
        private bool isAirborned = false;
        private float originShadowLocalY;


        public IEnumerator AirborneTo(Vector3 destination, float airborneHeight = 1f, float speed = 10f)
        {
            
            if (isAirborned)
                yield break;
            isAirborned = true;
            Vector3 origin = transform.position;
            if(shadow!=null)
            {
                originShadowLocalY = shadow.transform.localPosition.y;
            }
            else
                yield break;
            float currentAirborneHeight = 0f;
            
            Vector3 originalScale = transform.localScale;
            Vector3 originalShadowScale = shadow.transform.localScale;
            
            shadow.transform.parent = null;
            
            float duration = Vector3.Distance(origin, destination) / speed;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                if (CheckWallImpact())
                {
                    yield return OnWallImpact(duration - elapsed, originalScale, originalShadowScale, airborneHeight, originShadowLocalY, currentAirborneHeight);
                    yield break;
                }
                
                shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, originShadowLocalY - currentAirborneHeight, shadow.transform.localPosition.z);
                elapsed += Time.deltaTime;
                
                currentAirborneHeight = CalculateAirborneHeight(elapsed / duration, airborneHeight);

                UpdateScale(gameObject, originalScale, currentAirborneHeight/airborneHeight);
                UpdateScale(shadow, originalShadowScale, currentAirborneHeight/airborneHeight);
                
                shadow.transform.position = Vector3.Lerp(origin, destination, elapsed / duration) + Vector3.up * originShadowLocalY;
                transform.position = Vector3.Lerp(origin, destination, elapsed / duration) + Vector3.up * currentAirborneHeight;
                
                yield return null;
            }
            
            UpdateScale(gameObject, originalScale, 0);
            UpdateScale(shadow, originalShadowScale, 0);
            
            shadow.transform.parent = gameObject.transform;
            shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, originShadowLocalY, shadow.transform.localPosition.z);
            transform.position = destination;
            isAirborned = false;
        }
        
        private void UpdateScale(GameObject gameObject, Vector3 originalScale, float rate)
        {
            gameObject.transform.localScale = new Vector3(originalScale.x * (1+rate * 0.25f), originalScale.y * (1+rate * 0.25f), originalScale.z);
        }

        private bool CheckWallImpact()
        {
            Collider2D collider2D = Physics2D.OverlapCircle(transform.position, GetComponentInChildren<CircleCollider2D>().radius, LayerMask.GetMask("Wall"));
            return collider2D != null;
        }
        
        private IEnumerator OnWallImpact(float duration, Vector3 originalScale, Vector3 originalShadowScale, float airborneHeight, float originShadowLocalY, float currentAirborneHeight)
        {
            Vector3 origin = transform.position;
            Vector3 destination = transform.position + Vector3.down * currentAirborneHeight;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                UpdateScale(gameObject, originalScale, currentAirborneHeight/airborneHeight);
                UpdateScale(shadow, originalShadowScale, currentAirborneHeight/airborneHeight);
                
                transform.position = Vector3.Lerp(origin, destination, elapsed / duration) + Vector3.up * currentAirborneHeight;

                yield return null;
            }
            
            UpdateScale(gameObject, originalScale, 0);
            UpdateScale(shadow, originalShadowScale, 0);
            
            shadow.transform.parent = gameObject.transform;
            shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, originShadowLocalY, shadow.transform.localPosition.z);
            isAirborned = false;
        }

        private float CalculateAirborneHeight(float rate, float airborneHeight)
        {
            return Mathf.Sin(Mathf.PI * rate) * airborneHeight;
        }
    }
}
