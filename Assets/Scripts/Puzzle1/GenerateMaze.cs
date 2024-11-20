using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile floorTile;
    [SerializeField] GameObject shadowCasterPrefab;

    private int width = 49;
    private int height = 49;
    private bool[,] gridTile;
    private Stack<Vector2Int> pathStack = new();

    private Vector2Int start; 

    private void Start()
    {
        gridTile = new bool[width, height];
        start = new Vector2Int(1, height - 2); // 왼쪽 위(시작지점 or 끝지점)

        InitializeWalls();
        GenerateMap(start);
        GenerateTile(gridTile);
    }

    private void GenerateMap(Vector2Int start)
    {
        pathStack.Push(start);
        gridTile[start.x, start.y] = true;

        while (pathStack.Count > 0)
        {
            Vector2Int current = pathStack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                pathStack.Push(chosen);

                Vector2Int wallToRemove = (current + chosen) / 2;
                gridTile[wallToRemove.x, wallToRemove.y] = true;
                gridTile[chosen.x, chosen.y] = true;
            }
            else
            {
                pathStack.Pop();
            }
        }

        gridTile[width - 2, 0] = true;
        gridTile[1, height - 1] = true;
    }

    private void InitializeWalls()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridTile[i, j] = false;
            }
        }
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new();

        if (cell.x - 2 >= 0 && !gridTile[cell.x - 2, cell.y]) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.x + 2 < width && !gridTile[cell.x + 2, cell.y]) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.y - 2 >= 0 && !gridTile[cell.x, cell.y - 2]) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
        if (cell.y + 2 < height && !gridTile[cell.x, cell.y + 2]) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

        return neighbors;
    }

    private void GenerateTile(bool[,] gridTile)
    {
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();

        for (int i = 0; i < gridTile.GetLength(0); i++)
        {
            for (int j = 0; j < gridTile.GetLength(1); j++)
            {
                Vector3Int tilePosition = new(i - width / 2, j - height / 2, 0);

                if (gridTile[i, j])
                {
                    floorTilemap.SetTile(tilePosition, floorTile);
                }
                else
                {
                    wallTilemap.SetTile(tilePosition, wallTile);
                    CreateShadowCaster(tilePosition);
                }
            }
        }
    }

    private void CreateShadowCaster(Vector3Int tilePosition)
    {
        // 타일맵의 월드 좌표 계산
        Vector3 worldPosition = wallTilemap.GetCellCenterWorld(tilePosition);

        // Shadow Caster 프리팹 생성
        GameObject shadowCaster = Instantiate(shadowCasterPrefab, worldPosition, Quaternion.identity);

        // 그림자 오브젝트의 부모를 설정해 계층 정리
        shadowCaster.transform.SetParent(wallTilemap.transform);

        // 타일 크기에 맞게 조정
        shadowCaster.transform.localScale = wallTilemap.cellSize;
    }
}
