using System.Collections.Generic;
using UnityEngine;

namespace System.Game.QuestSystem.Quests
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
        [SerializeField] private int _questID;
        [SerializeField] private string _questName;
        [SerializeField] private string _questDescription;
        [SerializeField] private QuestType _questType;
        [SerializeField] private List<Goal> _goals;
        
        public SerializableQuestInfo(QuestInfo questInfo)
        {
            _questID = questInfo.questID;
            _questName = questInfo.questName;
            _questDescription = questInfo.questDescription;
            _questType = questInfo.questType;
            _boolString = "";
            _goals = new List<Goal>();
            foreach (Goal goal in questInfo.goals)
            {
                _goals.Add(new Goal() {goalName = goal.goalName, goalDescription = goal.goalDescription});
                _boolString += goal.isCompleted ? "1" : "0";
            }
        }

        public static implicit operator QuestInfo(SerializableQuestInfo serializableQuestInfo)
        {
            QuestInfo questInfo = ScriptableObject.CreateInstance<QuestInfo>();
            questInfo.questID = serializableQuestInfo._questID;
            questInfo.questName = serializableQuestInfo._questName;
            questInfo.questDescription = serializableQuestInfo._questDescription;
            questInfo.questType = serializableQuestInfo._questType;

            int index = 0;
            questInfo.goals = new List<Goal>();
            foreach (Goal goal in serializableQuestInfo._goals)
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
    
    public class Goal // Quest's Goal
    {
        public string goalName;
        public string goalDescription;
        public bool isCompleted;
    }
}