using playerCharacter;
using System.Collections;
using UnityEngine;

public class CreateRuneEvent : Event
{
    [SerializeField] Vector3 createPos;
    [SerializeField] int runeID;
    
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        RuneObjectCreator.Instance.DrawRuneObject(runeID, createPos);
        yield return null;
    }
}

public class AcquireRuneEvent : Event
{
    [SerializeField] public int runeID;

    public override IEnumerator Execute(EventObject eventObject = null)
    {
        RuneData runeData = RuneObjectCreator.Instance.runeDataList.GetRuneData(runeID);

        PlayerCharacter.Instance.GetComponent<RuneComponent>().AcquireRune(runeData);
        yield return null;
    }
}

// PrefabCreator..?
public class RuneObjectCreator : SingletonObject<RuneObjectCreator>
{
    [SerializeField] public RuneDataList runeDataList;
    [SerializeField] GameObject runeGameObject;

    public GameObject DrawRuneObject(int _runeID, Vector3 pos)
    {
        GameObject runeObj = Instantiate(runeGameObject, pos, Quaternion.identity);
        RuneObject rune = runeObj.GetComponent<RuneObject>();
        EventObject eventObj = runeObj.GetComponent<EventObject>();
        AcquireRuneEvent acquireRuneEvent = eventObj.EventSequence[0] as AcquireRuneEvent;
        acquireRuneEvent.runeID = _runeID;
        rune.TryInit(runeDataList.GetRuneData(_runeID));
        return runeObj;
    }
    
    private void CreatePuzzle1Rune()
    {
        DrawRuneObject(2, new Vector3(0,0,0));
    }
    
    private void Start()
    {
        CreatePuzzle1Rune();
    }
}
