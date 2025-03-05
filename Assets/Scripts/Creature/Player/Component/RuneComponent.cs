using System.Collections;
using System.Collections.Generic;
using System.Game.Rune.RuneUI;
using UnityEngine;

public class RuneComponent : MonoBehaviour
{

    [SerializeField] List<RuneData> equippedRuneList = new List<RuneData>();
    [SerializeField] List<RuneData> acquiredRuneList = new List<RuneData>();
    int maxRuneEquipNum = 2;

    private BuffComponent _buffComp = new BuffComponent();
    
    public List<RuneData> EquippedRuneList {  get { return equippedRuneList; } }
    public List<RuneData> AcquiredRuneList { get { return acquiredRuneList; } }
    public int MaxRuneEquipNum {  get { return maxRuneEquipNum; } }

    private List<IEnumerator> _activatingCoroutines = new();

    public void EquipRune(RuneData runeData)
    {
        if(equippedRuneList.Count < maxRuneEquipNum) 
        {
            equippedRuneList.Add(runeData);
            //효과 발동
            switch (runeData.id)
            {
                case 0:
                    IEnumerator _enumerator = RuneHitman.Instance.Activate_Breeze();
                    _activatingCoroutines.Add(_enumerator);
                    StartCoroutine(_enumerator);         
                    break;
                case 1:
                    RuneHitman.Instance.Activate_SwordAuraExercise();
                    break;
            }
        }
        else
        {
            Debug.LogError("There's no space to equip rune!");
        }
    }

    public void UnequipRune(RuneData runeData)
    {
        equippedRuneList.RemoveAll(x=>x.id == runeData.id);
        StopCoroutine(_activatingCoroutines[0]);
        _activatingCoroutines.Remove(_activatingCoroutines[0]);
        // _buffComp.DeleteBuff(runeData.runeBuff);
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

