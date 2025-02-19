using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.Puzzle.Maze
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap floorTilemap;
        [SerializeField] Tilemap thornTilemap;
        [SerializeField] Tile wallTile;
        [SerializeField] Tile shortWallTile;
        [SerializeField] Tile floorTile;
        [SerializeField] Tile thornTile;
        [SerializeField] GameObject shadowCasterPrefab;

        private int _width = 49;
        private int _height = 49;
        private bool[,] _gridTile;
        private bool[,] _thornGridTile;
        private bool _isDeadEnd = true;
        private Stack<Vector2Int> _pathStack = new();

        private Vector2Int _start; 

        private void Start()
        {
            _gridTile = new bool[_width, _height];
            _thornGridTile = new bool[_width, _height];
            _start = new Vector2Int(1,_height / 2 + 1); // 왼쪽 위(시작지점 or 끝지점)

            InitializeWalls();
            GenerateMap(_start);
            GenerateTile(_gridTile);
        }

        private void GenerateMap(Vector2Int start)
        {
            _pathStack.Push(start);
            _gridTile[start.x, start.y] = true;

            while (_pathStack.Count > 0)
            {
                Vector2Int current = _pathStack.Peek();
                List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

                if (neighbors.Count > 0)
                {
                    Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                    _pathStack.Push(chosen);

                    Vector2Int wallToRemove = (current + chosen) / 2;
                    _gridTile[wallToRemove.x, wallToRemove.y] = true;
                    _gridTile[chosen.x, chosen.y] = true;

                    _isDeadEnd = false;
                }
                else
                {
                    if (!_isDeadEnd && !IsInCenter(new Vector2Int(current.x, current.y)))
                    {
                        _thornGridTile[current.x, current.y] = true;
                        _isDeadEnd = true;
                    }
                    _pathStack.Pop();
                }
            }

            _gridTile[_width - 1, _height / 2 + 1] = true;
            _gridTile[0, _height / 2 + 1] = true;
            _thornGridTile[_width - 2, _height / 2 + 1] = false;
            _thornGridTile[1, _height/ 2 + 1] = false;
        }

        private void InitializeWalls()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _gridTile[i, j] = false;
                    _thornGridTile[i, j] = false;
                }
            }
        }

        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
        {
            List<Vector2Int> neighbors = new();

            if (cell.x - 2 >= 0 && !_gridTile[cell.x - 2, cell.y] && !IsInCenter(new Vector2Int(cell.x - 2, cell.y))) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
            if (cell.x + 2 < _width && !_gridTile[cell.x + 2, cell.y] && !IsInCenter(new Vector2Int(cell.x + 2, cell.y))) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
            if (cell.y - 2 >= 0 && !_gridTile[cell.x, cell.y - 2] && !IsInCenter(new Vector2Int(cell.x, cell.y - 2))) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
            if (cell.y + 2 < _height && !_gridTile[cell.x, cell.y + 2] && !IsInCenter(new Vector2Int(cell.x, cell.y + 2))) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

            return neighbors;
        }

        private bool IsInCenter(Vector2Int position)
        {
            int centerX = _width / 2;
            int centerY = _height / 2;
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
                    Vector3Int tilePosition = new(i - _width / 2, j - _height / 2, 0);
                    if (IsInCenter(new Vector2Int(i,j))) gridTile[i,j] = true;

                    if (gridTile[i, j])
                    {
                        floorTilemap.SetTile(tilePosition, floorTile);
                    }
                    else
                    {
                        if (ConditionManager.Instance.Conditions.ContainsKey("spring_puzzle1_clear"))
                        {
                            wallTilemap.SetTile(tilePosition, shortWallTile);
                        }
                        else
                        {
                            wallTilemap.SetTile(tilePosition, wallTile);
                            CreateShadowCaster(tilePosition);
                        } 
                    }
                }
            }

            for (int i = 0; i < _thornGridTile.GetLength(0); i++)
            {
                for (int j = 0; j < _thornGridTile.GetLength(1); j++)
                {
                    Vector3Int tilePosition = new(i - _width / 2, j - _height / 2, 0);

                    if (_thornGridTile[i, j]) thornTilemap.SetTile(tilePosition, thornTile);
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
}
