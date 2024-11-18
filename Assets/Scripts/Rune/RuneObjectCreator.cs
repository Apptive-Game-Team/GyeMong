using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PrefabCreator..?
public class RuneObjectCreator : MonoBehaviour
{
    [SerializeField] RuneDataList runeDataList;
    [SerializeField] GameObject runeGameObject;

    public GameObject DrawRuneObject(int runeID, Vector3 pos)
    {
        GameObject runeObj = Instantiate(runeGameObject, pos, Quaternion.identity);
        RuneObject rune = runeObj.GetComponent<RuneObject>();
        rune.TryInit(runeDataList.GetRuneData(runeID));
        return runeObj;
    }

    public void DoTest()
    {
        DrawRuneObject(3, new Vector3(-2,-2,0));
    }

    private void Start()
    {
        DoTest();
    }
}
