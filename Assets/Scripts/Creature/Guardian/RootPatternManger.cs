using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPatternManger : MonoBehaviour
{
    [SerializeField] private GameObject rootPrefab;
    private GameObject[] rootObjects;
    [SerializeField] private List<GameObject> rootSpawnZone;
    private void Awake()
    {
        gameObject.SetActive(false);
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
        while(true)
        {
            Debug.Log("°¡½Ã ¹Ù´Ú Áø");
            int caseNumber = Random.Range(1, 5);
            if (!rootObjects[0].activeSelf && !rootObjects[3].activeSelf)
            {
                switch (caseNumber)
                {
                    case 1:
                        ActivateRootObjects(new int[] { 0, 1, 7, 8, 9, 10, 11 });
                        break;
                    case 2:
                        ActivateRootObjects(new int[] { 1, 2, 3, 5, 9, 11, 12, 13 });
                        break;
                    case 3:
                        ActivateRootObjects(new int[] { 0, 2, 4, 6, 8, 10, 12, 14 });
                        break;
                    case 4:
                        ActivateRootObjects(new int[] { 0, 1, 3, 6, 8, 9, 11, 12, 14 });
                        break;
                }
                yield return new WaitForSeconds(2f);
            }
        }
    }
    private void ActivateRootObjects(int[] indices)
    {
        foreach (int index in indices)
        {
            rootObjects[index].SetActive(true);
        }
    }
}
