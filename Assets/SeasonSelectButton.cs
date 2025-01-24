using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonSelectButton : SelectableUI
{
    [SerializeField] private int seasonIndex;
    public override void OnInteract()
    {
        RuneWindow.Instance.ChangeRuneDataList(seasonIndex);
    }
}
