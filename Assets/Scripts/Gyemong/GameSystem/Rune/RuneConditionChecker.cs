using System.Collections.Generic;
using Util;

namespace Gyemong.GameSystem.Rune
{
    public enum RuneConditionType
    {
        DEFAULT,
        HAS_BUFF,
        STAT_CONDITION,
    }

    public enum RuneActionType
    {
        DEFAULT,
        ADD_BUFF,
        MODIFY_STAT,
    }

    public class RuneCondition
    {
        List<ConditionEntry> _conditionList = new List<ConditionEntry>();
        //Action like heal-player-0.01*maxhealth, addbuff-cooldown-3s, etc.
    
    }

//Condition Like if player-health<0.2*maxHealth, player-hasbuff-cooldown, etc.
    public class ConditionEntry
    {
        public Creature.Creature target;
        public bool condition;
    }

    public class RuneConditionChecker : SingletonObject<RuneConditionChecker>
    {
        List<RuneCondition> runeConditionList = new List<RuneCondition>();
    

        private void Update()
        {
            //checkRuneCondition
            foreach (var runeCondition in runeConditionList)
            {
            }
        }

        public void AddRuneCondition(RuneCondition runeCondition)
        {
            runeConditionList.Add(runeCondition);
        }
    }
}