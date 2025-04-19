using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CustomTiles/MazeWallTile")]
public class MazeWallTile : RuleTile<MazeWallTile>
{
    [SerializeField] private List<TileBase> singleTile;
    [SerializeField] private List<TileBase> horizontalLeftTile;
    [SerializeField] private List<TileBase> horizontalRightTile;
    [SerializeField] private List<TileBase> verticalBottomTile;
    [SerializeField] private List<TileBase> verticalTopTile;
    
    public TileType tileType = TileType.Single;
    public TileType setType = TileType.None;
    public bool forCheck = false;
    
    public enum TileType
    {
        None, Single, HorizontalLeft, HorizontalRight, VerticalBottom, VerticalTop
    }
    
    private Dictionary<TileType, List<TileBase>> tileMap;

    private void InitializeTileMap()
    {
        if (tileMap == null)
        {
            tileMap = new Dictionary<TileType, List<TileBase>>
            {
                { TileType.Single, singleTile },
                { TileType.HorizontalLeft, horizontalLeftTile },
                { TileType.HorizontalRight, horizontalRightTile },
                { TileType.VerticalBottom, verticalBottomTile },
                { TileType.VerticalTop, verticalTopTile }
            };
        }
    }
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        InitializeTileMap();
        
        if (setType != TileType.None)
        {
            tileType = setType;
            setType = TileType.None;
            tileData.sprite = GetTileSprite(tileMap[tileType][Random.Range(0, tileMap[tileType].Count)]);
            tileData.colliderType = Tile.ColliderType.Grid;
            Debug.Log("set by setter " + position);
            return;
        }
        
        if (forCheck)
        {
            forCheck = false;
            return;
        }
        
        tileType = TileType.Single;
        
        MazeWallTile left = tilemap.GetTile<MazeWallTile>(position + Vector3Int.left);
        MazeWallTile bottom = tilemap.GetTile<MazeWallTile>(position + Vector3Int.down);

        { // Rules
            if (left?.tileType == TileType.Single)
            {
                left.setType = TileType.HorizontalLeft;
                tileType = TileType.HorizontalRight;
            }
            if (bottom?.tileType == TileType.Single)
            {
                bottom.setType = TileType.VerticalBottom;
                tileType = TileType.VerticalTop;
            }
            
        }
        
        tileData.sprite = GetTileSprite(tileMap[tileType][Random.Range(0, tileMap[tileType].Count)]);
        tileData.colliderType = Tile.ColliderType.Grid;
    }
    
    private Sprite GetTileSprite(TileBase tileBase)
    {
        if (tileBase is Tile tile)
        {
            return tile.sprite;
        }
        return null;
    }
    
}
