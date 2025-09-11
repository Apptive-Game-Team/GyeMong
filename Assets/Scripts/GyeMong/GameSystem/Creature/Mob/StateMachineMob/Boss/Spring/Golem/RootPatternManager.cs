using System.Collections;
using System.Collections.Generic;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public enum MapPattern
    {
        Row,
        TwoRows,
        Column,
        TwoColumns,
        XCross,
        Rectangle
    }

    public class RootPatternManager : MonoBehaviour
    {
        [SerializeField] private GameObject rootPrefab;
        [SerializeField] private List<Transform> rootSpawnZone;

        [SerializeField] private int rows = 5;   // 행 개수
        [SerializeField] private int cols = 9;   // 열 개수

        private GameObject[,] rootObjects2D;

        private void Awake()
        {
            gameObject.SetActive(false);

            rootObjects2D = new GameObject[rows, cols];
            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GameObject root = Instantiate(rootPrefab, rootSpawnZone[index].position, Quaternion.identity);
                    root.SetActive(false);
                    rootObjects2D[r, c] = root;
                    index++;
                }
            }
        }

        private void OnEnable()
        {
            StartCoroutine(RootPattern());
        }

        private IEnumerator RootPattern()
        {
            MapPattern[] patterns = new MapPattern[]
            {
                MapPattern.Row,
                MapPattern.TwoRows,
                MapPattern.Column,
                MapPattern.TwoColumns,
                MapPattern.XCross,
                MapPattern.Rectangle
            };

            while (true)
            {
                MapPattern selectedPattern = patterns[Random.Range(0, patterns.Length)];
                ActivatePattern(selectedPattern);

                yield return new WaitForSeconds(2f);

                DeActivateAll();
            }
        }

        private void ActivatePattern(MapPattern pattern)
        {
            Sound.Play("ENEMY_Root");

            switch (pattern)
            {
                case MapPattern.Row:
                    int randomRow = Random.Range(0, rows);
                    for (int c = 0; c < cols; c++)
                        rootObjects2D[randomRow, c].SetActive(true);
                    break;

                case MapPattern.TwoRows:
                    int row1 = Random.Range(0, rows);
                    int row2;
                    do { row2 = Random.Range(0, rows); } while (row2 == row1);
                    for (int c = 0; c < cols; c++)
                    {
                        rootObjects2D[row1, c].SetActive(true);
                        rootObjects2D[row2, c].SetActive(true);
                    }
                    break;

                case MapPattern.Column:
                    int randomCol = Random.Range(0, cols);
                    for (int r = 0; r < rows; r++)
                        rootObjects2D[r, randomCol].SetActive(true);
                    break;

                case MapPattern.TwoColumns:
                    int col1 = Random.Range(0, cols);
                    int col2;
                    do { col2 = Random.Range(0, cols); } while (col2 == col1);
                    for (int r = 0; r < rows; r++)
                    {
                        rootObjects2D[r, col1].SetActive(true);
                        rootObjects2D[r, col2].SetActive(true);
                    }
                    break;

                case MapPattern.XCross:
                    for (int i = 0; i < Mathf.Min(rows, cols); i++)
                    {
                        rootObjects2D[i, i].SetActive(true);                   
                        rootObjects2D[i, cols - 1 - i].SetActive(true);       
                    }
                    break;

                case MapPattern.Rectangle:
                    for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        if (r == 0 || r == rows - 1 || c == 0 || c == cols - 1)
                            rootObjects2D[r, c].SetActive(true);
                    break;
            }
        }

        public void DeActivateAll()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    rootObjects2D[r, c].SetActive(false);
        }
    }
}
