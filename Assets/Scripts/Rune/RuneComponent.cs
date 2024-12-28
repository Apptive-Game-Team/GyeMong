using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneComponent : MonoBehaviour
{

    [SerializeField] List<RuneData> equippedRuneList = new List<RuneData>();
    [SerializeField] List<RuneData> acquiredRuneList = new List<RuneData>();
    int maxRuneEquipNum = 2;

    private BuffComponent _buffComp;
    
    public List<RuneData> EquippedRuneList {  get { return equippedRuneList; } }
    public List<RuneData> AcquiredRuneList { get { return acquiredRuneList; } }
    public int MaxRuneEquipNum {  get { return maxRuneEquipNum; } }

    private void Start()
    {
        _buffComp = GetComponent<BuffComponent>();
        TestAcquire();
    }

    private void TestAcquire()
    {
        RuneDataList dataList = RuneObjectCreator.Instance.runeDataList;
        AcquireRune(dataList.GetRuneData(1));
        AcquireRune(dataList.GetRuneData(2));
        AcquireRune(dataList.GetRuneData(3));
    }
    
    private void Update()
    {
        if(InputManager.Instance.GetKeyDown(ActionCode.RunePage))
        {
            RuneWindow.Instance.OpenOrCloseOption();
        }
    }

    public void EquipRune(RuneData runeData)
    {
        if(equippedRuneList.Count < maxRuneEquipNum) 
        {
            equippedRuneList.Add(runeData);
            _buffComp.AddBuff(runeData.runeBuff);
        }
        else
        {
            Debug.LogError("There's no space to equip rune!");
        }
    }

    public void UnequipRune(RuneData runeData)
    {
        equippedRuneList.Remove(runeData);
        _buffComp.DeleteBuff(runeData.runeBuff);
    }

    public void AcquireRune(RuneData runeData)
    {
        acquiredRuneList.Add(runeData);
    }

    public bool isRune(int id)
    {
        return equippedRuneList.Exists(x => x.id.Equals(id));
    }
}

