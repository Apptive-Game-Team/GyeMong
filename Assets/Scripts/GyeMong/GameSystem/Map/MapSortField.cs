using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapSortField : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")) sr.sortingOrder = 11;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")) sr.sortingOrder = 13;
    }
}
