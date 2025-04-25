using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Util;
using Util.QuestSystem.Quests;

namespace System.Event.Event.Condition
{
    public static class ConditionKeyRepository
    {

        public static List<string> ConditionKeys = new List<string>( );
        
        public static void UpdateConditionKeysFromQuest()
        { 
            List<QuestInfo> quests = ScriptableObjectFinder.FindAllScriptableObjects<QuestInfo>();
            foreach (QuestInfo quest in quests)
            {
                ConditionKeys.Add(quest.questName + "IsStarted");
                ConditionKeys.Add(quest.questName + "IsCleared");
            }
        }
    }
}