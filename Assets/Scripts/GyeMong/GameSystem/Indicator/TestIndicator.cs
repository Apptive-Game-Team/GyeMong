using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public class TestIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject _attackObject;
        
        private void Start()
        {
            StartCoroutine(Attack(_attackObject, new Vector2(-2f, 0), Quaternion.Euler(0,0,90)));
        }

        private IEnumerator Attack(GameObject attackObject, Vector2 pos, Quaternion rot)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                IndicatorGenerator.Instance.GenerateIndicator(attackObject, pos, rot, 0.5f);
                //yield return new WaitForSeconds(0.5f);
                var obj = Instantiate(attackObject, pos, rot);
                Destroy(obj, 0.5f);
            }
        }
    }
}
