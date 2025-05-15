using System.Collections;
using System.Collections.Generic;
using System.Game.Buff;
using System.Game.Rune;
using System.Game.Rune.RuneUI;
using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

public class RuneComponent : MonoBehaviour
{

    [SerializeField] List<RuneData> equippedRuneList = new List<RuneData>();
    [SerializeField] List<RuneData> acquiredRuneList = new List<RuneData>();
    int maxRuneEquipNum = 2;
    
    public List<RuneData> EquippedRuneList {  get { return equippedRuneList; } }
    public List<RuneData> AcquiredRuneList { get { return acquiredRuneList; } }
    public int MaxRuneEquipNum {  get { return maxRuneEquipNum; } }

    private List<IEnumerator> _activatingCoroutines = new();

    
    //추상화에 대한 issue는 다음 이슈에서 다루자.
    public void EquipRune(RuneData runeData)
    {
        if(equippedRuneList.Count < maxRuneEquipNum) 
        {
            equippedRuneList.Add(runeData);
            List<StatSet> statSetList = new();
            //효과 발동
            switch (runeData.id)
            {
                case 0:
                    new AddBuffRune().OnActivate(new AddBuffContext(PlayerCharacter.Instance.buffComponent,BuffManager.Instance.breezeRuneData));
                    break;
                case 1:
                    statSetList = new()
                    {
                        new StatSet(StatType.SKILL_COEF, StatValueType.PERCENT_VALUE, 0.3f),
                    };
                    new ModifyStatRune().OnActivate(new ModifyStatContext(PlayerCharacter.Instance.stat, statSetList));
                    break;
                case 2:
                    statSetList = new ()
                    {
                        new StatSet(StatType.ATTACK_DELAY, StatValueType.PERCENT_VALUE, -0.2f),
                        new StatSet(StatType.MOVE_SPEED, StatValueType.PERCENT_VALUE, 0.2f),
                        new StatSet(StatType.DASH_COOLDOWN, StatValueType.PERCENT_VALUE, -0.2f),
                    };
                    new ModifyStatRune().OnActivate(new ModifyStatContext(PlayerCharacter.Instance.stat, statSetList));
                    break;
                case 3:
                    break;
                case 4:
                    statSetList = new()
                    {
                        new StatSet(StatType.GRAZE_GAIN_ON_GRAZE, StatValueType.PERCENT_VALUE, 0.3f),
                    };
                    new ModifyStatRune().OnActivate(new ModifyStatContext(PlayerCharacter.Instance.stat, statSetList));
                    break;
                case 5:
                    break;
                case 6:
                    new AddBuffRune().OnActivate(new AddBuffContext(PlayerCharacter.Instance.buffComponent,BuffManager.Instance.stoneArmorRuneData));
                    break;
                case 7:
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

    public void UpgradeRune(RuneData runeData, int upgradeNum)
    {
        runeData.SelectedOption = runeData.availableOptions[upgradeNum];
        PlayerCharacter.Instance.stat.SetStatValues(runeData.SelectedOption.statList);
    }
}

