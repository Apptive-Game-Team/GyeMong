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
        XCrossFlipped,
        Rectangle,
        ShrinkingRectangle,
        CrossAtPoint,
        Wave
    }

    public class RootPatternManager : MonoBehaviour
    {
        [SerializeField] private GameObject rootPrefab;
        [SerializeField] private List<Transform> rootSpawnZone;

        [SerializeField] private int rows = 5;
        [SerializeField] private int cols = 9;

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
                //MapPattern.XCross,
                //MapPattern.XCrossFlipped,
                MapPattern.Rectangle,
                MapPattern.ShrinkingRectangle,
                MapPattern.CrossAtPoint,
                MapPattern.Wave
            };

            while (true)
            {
                MapPattern selectedPattern = patterns[Random.Range(0, patterns.Length)];
                yield return StartCoroutine(GetPatternCoroutine(selectedPattern));

                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator GetPatternCoroutine(MapPattern pattern)
        {
            switch (pattern)
            {
                case MapPattern.Row:               return RowPattern();
                case MapPattern.TwoRows:           return TwoRowsPattern();
                case MapPattern.Column:            return ColumnPattern();
                case MapPattern.TwoColumns:        return TwoColumnsPattern();
                case MapPattern.XCross:            return XCrossPattern();
                case MapPattern.XCrossFlipped:     return XCrossFlippedPattern();
                case MapPattern.Rectangle:         return RectanglePattern();
                case MapPattern.ShrinkingRectangle:return ShrinkingRectanglePattern();
                case MapPattern.CrossAtPoint:      return CrossAtPointPattern();
                case MapPattern.Wave:              return WavePattern();
                default:                           return null;
            }
        }
        private IEnumerator WavePattern()
        {
            
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                    rootObjects2D[r, c].SetActive(true);
                Sound.Play("ENEMY_Root");
                yield return new WaitForSeconds(0.5f);
                if (c > 0)
                {
                    for (int r = 0; r < rows; r++)
                        rootObjects2D[r, c - 1].SetActive(false);
                }
            }
            for (int r = 0; r < rows; r++)
                rootObjects2D[r, cols - 1].SetActive(false);

            yield return null;
        }
        private IEnumerator RowPattern()
        {
            Sound.Play("ENEMY_Root");
            int randomRow = Random.Range(0, rows);
            for (int c = 0; c < cols; c++)
                rootObjects2D[randomRow, c].SetActive(true);
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator TwoRowsPattern()
        {
            Sound.Play("ENEMY_Root");
            int row1 = Random.Range(0, rows);
            int row2;
            do { row2 = Random.Range(0, rows); } while (row2 == row1);

            for (int c = 0; c < cols; c++)
            {
                rootObjects2D[row1, c].SetActive(true);
                rootObjects2D[row2, c].SetActive(true);
            }
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator ColumnPattern()
        {
            Sound.Play("ENEMY_Root");
            int randomCol = Random.Range(0, cols);
            for (int r = 0; r < rows; r++)
                rootObjects2D[r, randomCol].SetActive(true);
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator TwoColumnsPattern()
        {
            Sound.Play("ENEMY_Root");
            int col1 = Random.Range(0, cols);
            int col2;
            do { col2 = Random.Range(0, cols); } while (col2 == col1);

            for (int r = 0; r < rows; r++)
            {
                rootObjects2D[r, col1].SetActive(true);
                rootObjects2D[r, col2].SetActive(true);
            }
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator XCrossPattern()
        {
            Sound.Play("ENEMY_Root");
            for (int i = 0; i < Mathf.Min(rows, cols); i++)
            {
                rootObjects2D[i, i].SetActive(true);
                rootObjects2D[i, cols - 1 - i].SetActive(true);
            }
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator XCrossFlippedPattern()
        {
            Sound.Play("ENEMY_Root");
            for (int i = 0; i < Mathf.Min(rows, cols); i++)
            {
                rootObjects2D[rows - 1 - i, i].SetActive(true);
                rootObjects2D[i, i].SetActive(true);
            }
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator RectanglePattern()
        {
            Sound.Play("ENEMY_Root");
            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (r == 0 || r == rows - 1 || c == 0 || c == cols - 1)
                    rootObjects2D[r, c].SetActive(true);
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        private IEnumerator ShrinkingRectanglePattern()
        {
            int layers = Mathf.Min(rows, cols) / 2;

            for (int layer = 0; layer < layers; layer++)
            {
                Sound.Play("ENEMY_Root");
                for (int r = layer; r < rows - layer; r++)
                {
                    rootObjects2D[r, layer].SetActive(true);
                    rootObjects2D[r, cols - 1 - layer].SetActive(true);
                }
                for (int c = layer; c < cols - layer; c++)
                {
                    rootObjects2D[layer, c].SetActive(true);
                    rootObjects2D[rows - 1 - layer, c].SetActive(true);
                }

                yield return new WaitForSeconds(1f);
                DeActivateAll();
            }
        }

        private IEnumerator CrossAtPointPattern()
        {
            Sound.Play("ENEMY_Root");
            int randR = Random.Range(0, rows);
            int randC = Random.Range(0, cols);

            for (int r = 0; r < rows; r++)
                rootObjects2D[r, randC].SetActive(true);
            for (int c = 0; c < cols; c++)
                rootObjects2D[randR, c].SetActive(true);
            yield return new WaitForSeconds(1f);
            DeActivateAll();
        }

        public void DeActivateAll()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    rootObjects2D[r, c].SetActive(false);
        }
    }
}
