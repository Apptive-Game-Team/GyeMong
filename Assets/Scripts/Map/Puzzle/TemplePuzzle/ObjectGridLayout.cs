using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Puzzle.TemplePuzzle
{
    public class ObjectGridLayout : MonoBehaviour
    {
        public GameObject[] objectPrefabs;
        public GameObject ballPrefab;
        public int rows = 4;
        public int columns = 4;
        public float spacing = 2f;
        public string objectIndices;
        public GameObject defaultObject;

        void Start()
        {
            if (!ConditionManager.Instance.Conditions.GetValueOrDefault("SpringTemplePuzzleIsCleared", false))
                CreateGridLayout();
            else
                defaultObject.SetActive(true);
        }

        void CreateGridLayout()
        {
            int listIndex = 0;
            Vector3 startPosition = transform.position;

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

                            if (objectIndex == 0)
                            {
                                GameObject ball = Instantiate(ballPrefab);

                                ball.transform.position = new Vector3(xPos, yPos, 0f);
                                ball.transform.SetParent(transform);
                            }

                            obj.transform.SetParent(transform);
                        }

                        listIndex++;
                    }
                }
            }
        }
    }
}
