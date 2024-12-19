using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using playerCharacter;

public class Puzzle2 : MonoBehaviour
{
    private Collider2D[] paths;
    List<GameObject> roots;
    Tilemap fogTilemap;
    PlayerCharacter player;
    List<Collider2D> pathList;

    const int visionRange = 2;
    int curRootNum = 0;

    private void Start()
    {
        player = PlayerCharacter.Instance;
        paths = transform.Find("Path").GetComponentsInChildren<Collider2D>();
        fogTilemap = GameObject.Find("fogTilemap").GetComponent<Tilemap>();
    }

    private void Update()
    {
        UpdateFog();
    }

    void UpdateFog()
    {
        Vector3Int playerPos = fogTilemap.WorldToCell(player.transform.position);

        for(int i=-visionRange; i<= visionRange ;i++)
        {
            for(int j=-visionRange;j<=visionRange ;j++)
            {
                fogTilemap.SetTile(playerPos + new Vector3Int(i, j, 0), null);
            }
        }
    }
    
    void UpdateResetTime()
    {

    }

}
