using UnityEngine;

namespace Map.Puzzle.Maze
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] GameObject[] treePrefabs;
        [SerializeField] GameObject grid;
 
        private float mapMinX = -43;
        private float mapMaxX = 43;
        private float mapMinY = -27f;
        private float mapMaxY = 36;

        private float mazeMinX = -25f;
        private float mazeMaxX = 26f;
        private float mazeMinY = -23;
        private float mazeMaxY = 25;

        private float rewardMinX = -35;
        private float rewardMaxX = -25;
        private float rewardMinY = -7;
        private float rewardMaxY = 9;

        private float roadMinX = 0;
        private float roadMaxX = 43;
        private float roadMinY = -5;
        private float roadMaxY = 9;

        private float treePlacementChance = 0.35f;

        private Vector2Int offset = new Vector2Int(); 

        void Start()
        {   
            offset.x = (int)grid.transform.position.x;
            offset.y = (int)grid.transform.position.y;
            PlaceTrees();
        }

        void PlaceTrees()
        {
            for (int x = (int)mapMinX + offset.x; x <= (int)mapMaxX + offset.x; x++)
            {
                for (int y = (int)mapMinY + offset.y; y <= (int)mapMaxY + offset.y; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (IsWithinBounds(position, mazeMinX + offset.x, mazeMaxX+ offset.x, mazeMinY+ offset.y, mazeMaxY + offset.y) || 
                        IsWithinBounds(position, rewardMinX+ offset.x, rewardMaxX+ offset.x, rewardMinY + offset.y, rewardMaxY + offset.y) ||
                        IsWithinBounds(position, roadMinX + offset.x, roadMaxX + offset.x, roadMinY + offset.y, roadMaxY + offset.y))
                    {
                        continue;
                    }

                    if (Random.value < treePlacementChance)
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
