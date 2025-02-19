using System.Collections;
using playerCharacter;
using UnityEngine;
using UnityEngine.VFX;

namespace Map.Puzzle.FogMaze
{
    public class FogController : MonoBehaviour, IEventTriggerable
    {
        [SerializeField] private VisualEffect vfxRenderer;
    
        private const float DELTA_TIME = 0.02f;
        private IEnumerator ClearFog()
        {
            float timer = 0;
            float clearTime = 5f;
            float currentRadius = vfxRenderer.GetFloat("Radius");
            float targetRadius = 10;
            float deltaRadius = (targetRadius - currentRadius) * (DELTA_TIME/clearTime);
            while (timer < clearTime)
            {
                vfxRenderer.SetFloat("Radius", currentRadius += deltaRadius);
                yield return new WaitForSeconds(DELTA_TIME);
                timer += DELTA_TIME;
            }
        }

        private void Update()
        {
            vfxRenderer.SetVector3("ColliderPos", PlayerCharacter.Instance.transform.position);
        }

        public void Trigger()
        {
            StartCoroutine(ClearFog());
        }
    }
}
