using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Quest.Quests
{
    public enum QuestType
    {
        MAIN = 0,
        SUB = 1
    }
        
    [Serializable]
    [CreateAssetMenu(fileName = "new QuestInfo", menuName = "Quest/QuestInfo", order = 0)]
    public class QuestInfo : ScriptableObject
    {
        public int questID;
        public string questName;
        public string questDescription;
        public QuestType questType;
        public List<Goal> goals;
    }
        
    [Serializable]
    public struct Goal // Quest's Goal
    {
        public string goalName;
        public string goalDescription;
        public bool isCompleted;
    }
}