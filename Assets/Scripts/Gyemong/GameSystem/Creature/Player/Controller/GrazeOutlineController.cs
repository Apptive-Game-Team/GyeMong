using System.Collections;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Player.Controller
{
    public class GrazeOutlineController : MonoBehaviour
    {
        private const float fadeDelay = 0.06f;
        private Coroutine curCoroutine;


        private void Awake()
        {
            SetAlpha(0f); // Disappear Default
        }

        private void SetAlpha(float alpha)
        {
            Material material = GetComponent<Renderer>().material;
            material.SetFloat("_OutlineAlpha", alpha);
        }

        public void AppearAndFadeOut()
        {
            if (curCoroutine != null)
            {
                StopCoroutine(curCoroutine);
            }

            curCoroutine = StartCoroutine(AppearAndFadeOutCoroutine());
        }

        private IEnumerator AppearAndFadeOutCoroutine()
        {
            SetAlpha(1f);

            float curAlpha = 1f;

            while (curAlpha > 0)
            {
                curAlpha -= fadeDelay;
                yield return new WaitForSeconds(fadeDelay);
                SetAlpha(curAlpha);
            }

            SetAlpha(0f);
        }
    }
}
