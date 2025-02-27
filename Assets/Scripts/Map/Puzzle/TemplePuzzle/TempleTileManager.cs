using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TempleTileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TempleTileData> tileDataMap = new Dictionary<Vector3Int, TempleTileData>();

    private bool isAttached = false;
    private Transform player;
    public float rotationSpeed = 300f;

    void Start()
    {
        InitializeTiles();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = false;
        }
    }

    private void Update()
    {
        if (isAttached)
        {
            if (InputManager.Instance != null && InputManager.Instance.GetKeyDown(ActionCode.Interaction))
            {
                Vector3Int playerTilePos = tilemap.WorldToCell(player.position);
                RotateTile(playerTilePos);
            }
        }
    }

    void InitializeTiles()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                tileDataMap[pos] = GetTileData(tile);
            }
        }
    }

    TempleTileData GetTileData(TileBase tile)
    {
        switch (tile.name)
        {
            case "StraightTile": return new TempleTileData(true, true, false, false);
            case "CurveTile": return new TempleTileData(true, false, false, true);
            case "ThreeWayTile": return new TempleTileData(true, true, false, true);
            case "FourWayTile": return new TempleTileData(true, true, true, true);
            default: return new TempleTileData(false, false, false, false);
        }
    }

    public bool CanConnect(Vector3Int from, Vector3Int to)
    {
        if (!tileDataMap.ContainsKey(from) || !tileDataMap.ContainsKey(to))
            return false;

        TempleTileData fromTile = tileDataMap[from];
        TempleTileData toTile = tileDataMap[to];

        Vector3Int direction = to - from;

        if (direction == Vector3Int.up) return fromTile.up && toTile.down;
        if (direction == Vector3Int.down) return fromTile.down && toTile.up;
        if (direction == Vector3Int.left) return fromTile.left && toTile.right;
        if (direction == Vector3Int.right) return fromTile.right && toTile.left;

        return false;
    }

    public void RotateTile(Vector3Int pos)
    {
        if (!tileDataMap.ContainsKey(pos)) return;

        TempleTileData tile = tileDataMap[pos];
        TileBase currentTile = tilemap.GetTile(pos);

        Tile currentTileAsTile = currentTile as Tile;

        if (currentTileAsTile != null)
        {
            float currentRotation = currentTileAsTile.gameObject.transform.rotation.eulerAngles.z;
            currentTileAsTile.gameObject.transform.rotation = Quaternion.Euler(0, 0, currentRotation + 90);

            bool temp = tile.up;
            tile.up = tile.left;
            tile.left = tile.down;
            tile.down = tile.right;
            tile.right = temp;

            tilemap.RefreshTile(pos);
        }
    }
}
