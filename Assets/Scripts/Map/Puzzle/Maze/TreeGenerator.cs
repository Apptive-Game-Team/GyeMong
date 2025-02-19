using UnityEngine;

namespace Map.Puzzle.Maze
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] GameObject[] treePrefabs;
        [SerializeField] GameObject grid;
 
        private float _mapMinX = -43;
        private float _mapMaxX = 43;
        private float _mapMinY = -27f;
        private float _mapMaxY = 36;

        private float _mazeMinX = -25f;
        private float _mazeMaxX = 26f;
        private float _mazeMinY = -23;
        private float _mazeMaxY = 25;

        private float _rewardMinX = -35;
        private float _rewardMaxX = -25;
        private float _rewardMinY = -7;
        private float _rewardMaxY = 9;

        private float _roadMinX = 0;
        private float _roadMaxX = 43;
        private float _roadMinY = -5;
        private float _roadMaxY = 9;

        private float _treePlacementChance = 0.35f;

        private Vector2Int _offset = new Vector2Int(); 

        void Start()
        {   
            _offset.x = (int)grid.transform.position.x;
            _offset.y = (int)grid.transform.position.y;
            PlaceTrees();
        }

        void PlaceTrees()
        {
            for (int x = (int)_mapMinX + _offset.x; x <= (int)_mapMaxX + _offset.x; x++)
            {
                for (int y = (int)_mapMinY + _offset.y; y <= (int)_mapMaxY + _offset.y; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (IsWithinBounds(position, _mazeMinX + _offset.x, _mazeMaxX+ _offset.x, _mazeMinY+ _offset.y, _mazeMaxY + _offset.y) || 
                        IsWithinBounds(position, _rewardMinX+ _offset.x, _rewardMaxX+ _offset.x, _rewardMinY + _offset.y, _rewardMaxY + _offset.y) ||
                        IsWithinBounds(position, _roadMinX + _offset.x, _roadMaxX + _offset.x, _roadMinY + _offset.y, _roadMaxY + _offset.y))
                    {
                        continue;
                    }

                    if (Random.value < _treePlacementChance)
                    {
                        GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        Instantiate(treePrefab, position, Quaternion.identity,transform);
                    }
                }
            }
        }

        bool IsWithinBounds(Vector2 position, float minX, float maxX, float minY, float maxY)
        {
            return position.x >= minX && position.x <= maxX &&
                   position.y >= minY && position.y <= maxY;
        }
    }
}
