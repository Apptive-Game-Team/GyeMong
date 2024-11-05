using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject tilePrefab;

    private int width = 49;
    private int height = 49;

    private float tileRatio = 1f;
    private bool[,] gridTile;


    void Start()
    {
        Generatemap(width,height);
        GenerateTile(gridTile);
    }

    void Generatemap(int width, int height)
    {
        gridTile = new bool[width, height];

        for (int i = 0;i < width;i++) // 가장자리 벽 설정
        {
            for (int j = 0;j < height;j++) 
            {
                if (i % 2 == 0 || j % 2 == 0)
                {
                    gridTile[i,j] = false;
                }
                else gridTile[i, j] = true;
            }
        }

        for (int i = 0;i < width;i++)
        {
            for (int j = 0;j < height;j++)
            {
                if (i % 2 == 0 || j % 2 == 0) continue; // 가장자리는 그대로 벽
                if (i == width - 2 && j == height - 2) continue; // 마지막 타일 = 벽

                // if (i == width - 2) // i의 다음값이 가장자리(벽)일때
                // {
                //     gridTile[i,j+1] = true; // 다른방향으로 벽뚫음 (오른쪽)
                //     continue;
                // }

                // if (j == height - 2) // j의 다음값이 가장자리(벽)일때
                // {
                //     gridTile[i+1,j] = true; // 다른방향으로 벽뚫음 (아래쪽)
                //     continue;
                // }

                if (Random.Range(0, 2) == 0) // 랜덤으로 오른쪽 혹은 왼쪽으로 벽을 뚫는다.
                {
                    if (i < width - 2)
                    {
                        gridTile[i + 1, j] = true;
                    }
                }
                else
                {
                    if (j < height - 2)
                    {
                        gridTile[i, j + 1] = true;
                    }
                }
            }
        }
    }

    void GenerateTile(bool[,] gridTile)
    {
        for (int i = 0; i < gridTile.GetLength(0); i++)
        {
            for (int j = 0; j < gridTile.GetLength(1); j++)
            {
                Vector2 position = new Vector2((i - width / 2) * tileRatio, (j - height / 2) * tileRatio);

                if (!gridTile[i, j]) 
                {
                    Instantiate<GameObject>(wallPrefab, position, Quaternion.identity);
                }
                else 
                {
                    Instantiate<GameObject>(tilePrefab, position, Quaternion.identity);
                }
            }
        }
    }

}
