using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : SingletonObject<GoldManager>
{
    private int gold;

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Gold: " + gold);
    }

    public int GetGold()
    {
        return gold;
    }
}
