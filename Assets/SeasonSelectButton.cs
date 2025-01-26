using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonSelectButton : SelectableUI
{
    [SerializeField] private int seasonIndex;
    [SerializeField] private GameObject acquiredRuneListUI;
    public override void OnInteract()
    {
        for (int i = 0; i < acquiredRuneListUI.transform.childCount; i++)
        {
            acquiredRuneListUI.transform.GetChild(i).gameObject.SetActive(seasonIndex == i);
        } 
    }
}
