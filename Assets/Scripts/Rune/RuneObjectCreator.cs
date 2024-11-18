using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRuneEvent : Event
{
    [SerializeField] Vector3 createPos;
    [SerializeField] int runeID;

    public override IEnumerator execute()
    {
        RuneObjectCreator.Instance.DrawRuneObject(runeID, createPos);
        yield return null;
    }
}

public class AcquireRuneEvent : Event
{
    public override IEnumerator execute()
    {
        throw new System.NotImplementedException();
    }
}

// PrefabCreator..?
public class RuneObjectCreator : SingletonObject<RuneObjectCreator>
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
