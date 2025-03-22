using System.Collections;
using System.Collections.Generic;
using System.Game.Rune.RuneUI;
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

    public override void OnLongInteract()
    {
        throw new System.NotImplementedException();
    }
}
