using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public class TestIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject _attackObject1;
        [SerializeField] private GameObject _attackObject2;
        [SerializeField] private GameObject _attackObject3;
        
        private void Start()
        {
            StartCoroutine(Attack(_attackObject1, new Vector2(-2f, 0), Quaternion.Euler(0,0,90)));
            StartCoroutine(Attack(_attackObject2, new Vector2(-2f, -2f), Quaternion.Euler(0,0,45)));
            StartCoroutine(Attack(_attackObject3, new Vector2(-2f, -4f), Quaternion.Euler(0,0,60)));
        }

        private IEnumerator Attack(GameObject attackObject, Vector2 pos, Quaternion rot)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                IndicatorGenerator.Instance.GenerateIndicator(attackObject, pos, rot, 1f);
                yield return new WaitForSeconds(1f);
                var obj = Instantiate(attackObject, pos, rot);
                Destroy(obj, 0.5f);
            }
        }
    }
}
