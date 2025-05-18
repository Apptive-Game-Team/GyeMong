using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller.Condition;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public class RootPatternManger : MonoBehaviour
    {
        [SerializeField] private GameObject rootPrefab;
        private GameObject[] rootObjects;
        [SerializeField] private List<GameObject> rootSpawnZone;

        private SoundObject soundObject;

        private void Awake()
        {
            // if (ConditionManager.Instance.Conditions.TryGetValue("spring_guardian_down", out bool down))
            // {
            //     if (down)
            //     {
            //         Destroy(gameObject);
            //     }
            // }
            gameObject.SetActive(false);
            soundObject = GetComponent<SoundObject>();
        }
        void OnEnable()
        {
            rootObjects = new GameObject[rootSpawnZone.Count];
            for (int i = 0; i < rootSpawnZone.Count; i++)
            {
                GameObject rootObject = Instantiate(rootPrefab, rootSpawnZone[i].transform.position, Quaternion.identity);
                rootObjects[i] = rootObject;
                rootObject.SetActive(false);
            }
            StartCoroutine(RootPattern());
        }

        private IEnumerator RootPattern()
        {
            while (true)
            {
                int caseNumber = Random.Range(1, 3);
                if (!rootObjects[0].activeSelf && !rootObjects[3].activeSelf)
                {
                    switch (caseNumber)
                    {
                        case 1:
                            ActivateRootObjects(new int[] { 0, 2, 4, 6, 8, 10, 13, 15, 17, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38 });
                            break;
                        case 2:
                            ActivateRootObjects(new int[] { 1, 3, 5, 7, 9 , 11, 12, 14, 16, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37 });
                            break;
                    }
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        private void ActivateRootObjects(int[] indices)
        {
            StartCoroutine(soundObject.Play());
            foreach (int index in indices)
            {
                rootObjects[index].SetActive(true);
            }
        }
        public void DeActivateRootObjects()
        {
            StopAllCoroutines();
            foreach (GameObject gameObject in rootObjects)
            {
                gameObject.SetActive(false);
            }
        }
    }
}