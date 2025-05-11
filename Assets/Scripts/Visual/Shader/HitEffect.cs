using System.Collections;
using UnityEngine;

namespace Visual.Shader
{
    public class HitEffect : MonoBehaviour
    {
        public GameObject gameObjects;
        private Material material;
        private bool flag = true;
        private float blinkDelay = 0.3f;
        private float invincibleTime = 1f;

        private void Awake()
        {
            material = gameObjects.GetComponent<Renderer>().material;
        }

        public void OnClickButton()
        {   
            if (flag)
            {   
                flag = !flag;
                StartCoroutine(Blink());
                Debug.Log("tlqk");
            }
        }

        private IEnumerator Blink()
        {
            material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(invincibleTime - blinkDelay);
            flag = !flag;
        }
    }
}
