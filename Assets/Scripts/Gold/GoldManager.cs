using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;
    private int gold;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
