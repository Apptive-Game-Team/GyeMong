using System.Collections;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime;
using Unity.VisualScripting;
using UnityEngine;

namespace GyeMong.EventSystem.Event.EventScene
{
    public abstract class EventScene : Event { }

    public class SlimeEvents : EventScene
    {
        [SerializeField] private GameObject targetSlime;
        [SerializeField] private GameObject[] slimes;

        public SlimeEvents(GameObject targetSlime, GameObject[] slimes)
        {
            this.targetSlime = targetSlime;
            this.slimes = slimes;
        }

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            for (int i = 0;i < slimes.Length;i++)
            {
                GameObject randomSlime = slimes[i];
                randomSlime.SetActive(true);
                Vector2 randomDirection = Random.insideUnitCircle.normalized * 4f;
                Vector3 randomPosition = targetSlime.transform.position + (Vector3)randomDirection;
                randomSlime.transform.position = randomPosition;

                if (eventObject != null)
                {
                    eventObject.StartCoroutine(MoveSlimeToTarget(randomSlime));
                }
                else
                {
                    CoroutineRunner.Instance.StartCoroutine(MoveSlimeToTarget(randomSlime));
                }

                float randomTime = Random.Range(0.5f, 1f);
                yield return new WaitForSeconds(randomTime);
            }

            yield return new WaitForSeconds(3f);
            targetSlime.GetComponent<DivisionSlime>().StartMob();
            // targetSlime.AddComponent<DivisionSlime>();
        }

        private IEnumerator MoveSlimeToTarget(GameObject slime)
        {
            Vector3 startPosition = slime.transform.position;
            Vector3 targetPosition = targetSlime.transform.position;
            float duration = Random.Range(1.5f, 2.5f);
            float elapsedTime = 0f;
            float delay = 0.01f;
            Vector3 scale = slime.transform.localScale;
            slime.transform.localScale = startPosition.x < targetPosition.x ? scale : new Vector3(-scale.x, scale.y, scale.z);
            slime.SetActive(true);

            while (elapsedTime < duration)
            {
                elapsedTime += delay;
                float t = elapsedTime / duration;
                slime.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return new WaitForSeconds(delay);
            }
            slime.SetActive(false);

            targetSlime.transform.localScale *= 1.25f;
            yield return new WaitForSeconds(0.2f);
            targetSlime.transform.localScale *= 0.92f;
        }
    }
}