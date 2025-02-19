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

    public bool SpendGold(int amount)
    {
        if (gold - amount < 0)
        {
            Debug.Log("Not Enough Money");
            return false;
        }

        gold -= amount;
        return true;
    }

    public int GetGold()
    {
        return gold;
    }
}
