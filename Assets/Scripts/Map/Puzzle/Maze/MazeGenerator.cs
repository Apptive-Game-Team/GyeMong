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
        [SerializeField] RuleTile wallTile;
        [SerializeField] Tile floorTile;
        [SerializeField] Tile thornTile;

        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private bool _top;
        [SerializeField] private bool _bottom;
        [SerializeField] private bool _left;
        [SerializeField] private bool _right;
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
                    if (!_isDeadEnd)
                    {
                        _thornGridTile[current.x, current.y] = true;
                        _isDeadEnd = true;
                    }
                    _pathStack.Pop();
                }
            }

            if (_right) 
                _gridTile[_width - 1, _height / 2 + 1] = true;
                _thornGridTile[_width - 2, _height / 2 + 1] = false;
            if (_left) 
                _gridTile[0, _height / 2 + 1] = true;
                _thornGridTile[1, _height / 2 + 1] = false;
            if (_top) 
                _gridTile[_width / 2, _height - 1] = true;
                _gridTile[_width / 2 - 1, _height - 1] = true;
                _thornGridTile[_width / 2 + 1, _height - 2] = false;
            if (_bottom) 
                _gridTile[_width / 2, 0] = true;
                _gridTile[_width / 2 - 1, 0] = true;
                _thornGridTile[_width / 2 + 1, 1] = false;
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

            if (cell.x - 2 >= 0 && !_gridTile[cell.x - 2, cell.y]) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
            if (cell.x + 2 < _width && !_gridTile[cell.x + 2, cell.y]) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
            if (cell.y - 2 >= 0 && !_gridTile[cell.x, cell.y - 2]) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
            if (cell.y + 2 < _height && !_gridTile[cell.x, cell.y + 2]) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

            return neighbors;
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

                    if (gridTile[i, j])
                    {
                        floorTilemap.SetTile(tilePosition, floorTile);
                    }
                    else
                    {
                        wallTilemap.SetTile(tilePosition, wallTile);
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

        public void ReGenerateMaze(bool _top, bool _bottom, bool _left, bool _right)
        {
            this._top = _top;
            this._bottom = _bottom;
            this._left = _left;
            this._right = _right;

            InitializeWalls();
            GenerateMap(_start);
            GenerateTile(_gridTile);
        }
    }
}
