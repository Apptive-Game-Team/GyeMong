using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PrefabCreator..?
public class RuneObjectCreator : MonoBehaviour
{
    [SerializeField] RuneDataList runeDataList;


    public GameObject DrawRuneObject()
    {
        GameObject runeObj = Instantiate(gameObject);
        return runeObj;
    }
}
