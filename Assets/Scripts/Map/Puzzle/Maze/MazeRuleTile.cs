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

    private enum TileType
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

    private TileType GetTileType(TileBase tile)
    {
        if (horizontalLeftTile.Contains(tile)) return TileType.HorizontalLeft;
        if (horizontalRightTile.Contains(tile)) return TileType.HorizontalRight;
        if (verticalBottomTile.Contains(tile)) return TileType.VerticalBottom;
        if (verticalTopTile.Contains(tile)) return TileType.VerticalTop;
        if (singleTile.Contains(tile)) return TileType.Single;
        return TileType.None;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        InitializeTileMap();

        TileType left = GetTileType(tilemap.GetTile(position + Vector3Int.left));
        TileType bottom = GetTileType(tilemap.GetTile(position + Vector3Int.down));
        TileType bottomLeft = GetTileType(tilemap.GetTile(position + Vector3Int.down + Vector3Int.left));
        TileType topLeft = GetTileType(tilemap.GetTile(position + Vector3Int.up + Vector3Int.left));

        TileType resultType = TileType.Single;

        // if (bottom == TileType.None)
        // {
        //     if (left == TileType.None) resultType = TileType.Single;
        //     else if (left == TileType.Single)
        //     {
        //         if (topLeft == TileType.Single)
        //         else if (topLeft == TileType.HorizontalLeft) resultType = TileType.Single;
        //         else if (topLeft == TileType.HorizontalRight)
        //     }
        //     else if (left == TileType.HorizontalLeft) resultType = TileType.HorizontalRight;
        //     else if (left == TileType.HorizontalRight) resultType = TileType.Single;
        // }
        // else if (bottom == TileType.Single)
        // {
        //     resultType = TileType.VerticalBottom;
        // }
        // else if (bottom == TileType.VerticalBottom)
        // {
        //     if (left == TileType.HorizontalLeft) resultType = TileType.HorizontalRight;
        //     else if (left == TileType.HorizontalRight) resultType = TileType.HorizontalLeft;
        //     else if (left == TileType.None) resultType = TileType.VerticalTop;
        // }
        // else if (bottom == TileType.VerticalTop)
        // {
        //     if (left == TileType.HorizontalLeft) resultType = TileType.HorizontalRight;
        //     else if (left == TileType.HorizontalRight) resultType = TileType.HorizontalLeft;
        //     else if (left == TileType.None) resultType = TileType.Single;
        // }

        tileData.sprite = GetTileSprite(tileMap[resultType][Random.Range(0, tileMap[resultType].Count)]);
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
