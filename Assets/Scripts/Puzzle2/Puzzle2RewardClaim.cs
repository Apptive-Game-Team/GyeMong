using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Puzzle2RewardClaim : MonoBehaviour
{
    Tilemap fogTilemap;

    private void Start()
    {
        fogTilemap = GameObject.Find("fogTilemap").GetComponent<Tilemap>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        fogTilemap.gameObject.SetActive(false);
    }
}
