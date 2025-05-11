using System.Collections;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public class CubeShadow : MonoBehaviour
    {
        private GameObject player;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(FollowAndFall());
        }

        private IEnumerator FollowAndFall()
        {
            float followDuration = 1f;
            float elapsedTime = 0f;
            Color initialColor = spriteRenderer.color;
            spriteRenderer.color = initialColor;

            while (elapsedTime < followDuration)
            {
                if (player != null)
                {
                    transform.position = player.transform.position - new Vector3(0, 0.6f, 0);
                    float t = elapsedTime / followDuration;
                    Color newColor = spriteRenderer.color;
                    newColor.a = Mathf.Lerp(initialColor.a, 1f, t);
                    spriteRenderer.color = newColor;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Cube>() != null)
            {
                Destroy(gameObject);
            }
        }
    }
}