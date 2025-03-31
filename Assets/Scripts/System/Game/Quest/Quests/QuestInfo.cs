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
        [SerializeField] public int questID;
        [SerializeField] public string questName;
        [SerializeField] public string questDescription;
        [SerializeField] public QuestType questType;
        [SerializeField] public List<Goal> goals;
        
        public static implicit operator SerializableQuestInfo(QuestInfo questInfo)
        {
            return new SerializableQuestInfo(questInfo);
        }
    }

    [Serializable]
    public class SerializableQuestInfo
    {
        [SerializeField] private string _boolString;
        [SerializeReference] private QuestInfo _questInfo;
        
        public SerializableQuestInfo(QuestInfo questInfo)
        {
            _questInfo = questInfo;
            _boolString = "";
            foreach (Goal goal in questInfo.goals)
            {
                _boolString += goal.isCompleted ? "1" : "0";
            }
        }

        public static implicit operator QuestInfo(SerializableQuestInfo serializableQuestInfo)
        {
            QuestInfo questInfo = ScriptableObject.CreateInstance<QuestInfo>();
            questInfo.questID = serializableQuestInfo._questInfo.questID;
            questInfo.questName = serializableQuestInfo._questInfo.questName;
            questInfo.questDescription = serializableQuestInfo._questInfo.questDescription;
            questInfo.questType = serializableQuestInfo._questInfo.questType;

            int index = 0;
            questInfo.goals = new List<Goal>();
            foreach (Goal goal in serializableQuestInfo._questInfo.goals)
            {
                questInfo.goals.Add(
                    new Goal()
                    {
                        goalName = goal.goalName, 
                        goalDescription = goal.goalDescription, 
                        isCompleted = (serializableQuestInfo._boolString[index] == '1')
                    });
                index++;
            }

            return questInfo;
        }
    } 
        
    [Serializable]
    public struct Goal // Quest's Goal
    {
        public string goalName;
        public string goalDescription;
        public bool isCompleted;
    }
}