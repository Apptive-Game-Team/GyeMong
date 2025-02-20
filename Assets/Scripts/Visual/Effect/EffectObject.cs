using System.Collections;
using UnityEngine;

namespace Visual.Effect
{
    public class EffectObject : MonoBehaviour
    {
        EffectData effectData;

        public int ID
        {
            get
            {
                return effectData.id;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SetUnactive(0.5f));
        }
    
        private IEnumerator SetUnactive(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }
    }
}