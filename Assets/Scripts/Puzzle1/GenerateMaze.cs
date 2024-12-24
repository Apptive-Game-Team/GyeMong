using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap thornTilemap;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile thornTile;
    [SerializeField] GameObject shadowCasterPrefab;

    private int width = 49;
    private int height = 49;
    private bool[,] gridTile;
    private bool[,] thornGridTile;
    private bool isDeadEnd = true;
    private Stack<Vector2Int> pathStack = new();

    private Vector2Int start; 

    private void Start()
    {
        gridTile = new bool[width, height];
        thornGridTile = new bool[width, height];
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

                isDeadEnd = false;
            }
            else
            {
                if (!isDeadEnd && !IsInCenter(new Vector2Int(current.x, current.y)))
                {
                    thornGridTile[current.x, current.y] = true;
                    isDeadEnd = true;
                }
                pathStack.Pop();
            }
        }

        gridTile[width - 2, 0] = true;
        gridTile[1, height - 1] = true;
        thornGridTile[width - 2, 1] = false;
        thornGridTile[1, height - 2] = false;
    }

    private void InitializeWalls()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridTile[i, j] = false;
                thornGridTile[i, j] = false;
            }
        }
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new();

        if (cell.x - 2 >= 0 && !gridTile[cell.x - 2, cell.y] && !IsInCenter(new Vector2Int(cell.x - 2, cell.y))) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.x + 2 < width && !gridTile[cell.x + 2, cell.y] && !IsInCenter(new Vector2Int(cell.x + 2, cell.y))) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.y - 2 >= 0 && !gridTile[cell.x, cell.y - 2] && !IsInCenter(new Vector2Int(cell.x, cell.y - 2))) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
        if (cell.y + 2 < height && !gridTile[cell.x, cell.y + 2] && !IsInCenter(new Vector2Int(cell.x, cell.y + 2))) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

        return neighbors;
    }

    private bool IsInCenter(Vector2Int position)
    {
        int centerX = width / 2;
        int centerY = height / 2;
        int halfSize = 4;

        return position.x >= centerX - halfSize && position.x <= centerX + halfSize && position.y >= centerY - halfSize && position.y <= centerY + halfSize;
    }

    private void GenerateTile(bool[,] gridTile)
    {
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        thornTilemap.ClearAllTiles();

        for (int i = 0; i < gridTile.GetLength(0); i++)
        {
            for (int j = 0; j < gridTile.GetLength(1); j++)
            {
                Vector3Int tilePosition = new(i - width / 2, j - height / 2, 0);
                if (IsInCenter(new Vector2Int(i,j))) gridTile[i,j] = true;

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

        for (int i = 0; i < thornGridTile.GetLength(0); i++)
        {
            for (int j = 0; j < thornGridTile.GetLength(1); j++)
            {
                Vector3Int tilePosition = new(i - width / 2, j - height / 2, 0);

                if (thornGridTile[i, j]) thornTilemap.SetTile(tilePosition, thornTile);
            }
        }
    }

    private void CreateShadowCaster(Vector3Int tilePosition)
    {
        Vector3 worldPosition = wallTilemap.GetCellCenterWorld(tilePosition);
        GameObject shadowCaster = Instantiate(shadowCasterPrefab, worldPosition, Quaternion.identity);
        shadowCaster.transform.SetParent(wallTilemap.transform);
        shadowCaster.transform.localScale = wallTilemap.cellSize;
    }
}
