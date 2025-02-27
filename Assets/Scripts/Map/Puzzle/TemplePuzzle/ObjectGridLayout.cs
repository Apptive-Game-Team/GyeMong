using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGridLayout : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public int rows = 4;
    public int columns = 4;
    public float spacing = 2f;
    public string objectIndices;
    public Vector3 startPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        CreateGridLayout();
    }

    void CreateGridLayout()
    {
        int listIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (listIndex < objectIndices.Length)
                {
                    char currentChar = objectIndices[listIndex];
                    int objectIndex = int.Parse(currentChar.ToString());

                    if (objectIndex >= 0 && objectIndex < objectPrefabs.Length)
                    {
                        GameObject obj = Instantiate(objectPrefabs[objectIndex]);

                        float xPos = startPosition.x + (col * spacing);
                        float yPos = startPosition.y + (row * spacing);

                        obj.transform.position = new Vector3(xPos, yPos, 0f);

                        obj.transform.SetParent(transform);
                    }

                    listIndex++;
                }
            }
        }
    }
}
