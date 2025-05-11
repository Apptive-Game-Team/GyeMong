using System;
using Gyemong.EventSystem.Controller.Condition;
using UnityEngine;

namespace Gyemong.QuestSystem.Quests
{
    [Serializable]
    public class Quest
    {
        private String _path;
        public Quest(String path)
        {
            _path = path;
        }

        public Quest(QuestInfo questInfo)
        {
            _questInfo = questInfo;
        }
        [SerializeReference] private QuestInfo _questInfo;
        public QuestInfo GetQuestInfo
        {
            get
            {
                if (_questInfo == null)
                {
                    _questInfo = UnityEngine.Resources.Load<QuestInfo>(_path);
                }
                return _questInfo;
            }
        }

        public String StartConditionKey => GetQuestInfo.questName + "IsStarted";
        public String ClearConditionKey => GetQuestInfo.questName + "IsCleared";
        public int NumOfGoals => GetQuestInfo.goals.Count;
        public int NumOfCompletedGoals => GetQuestInfo.goals.FindAll(goal => goal.isCompleted).Count;
        public int NumOfUncompletedGoals => GetQuestInfo.goals.FindAll(goal => !goal.isCompleted).Count;
        public bool IsClear => NumOfGoals == NumOfCompletedGoals;

        public void StartQuest()
        {
            ConditionManager.Instance.Conditions[StartConditionKey] = true;
        }
        // public abstract void UpdateQuest(); // Quest Update Event
        public void EndQuest()
        {
            ConditionManager.Instance.Conditions[ClearConditionKey] = true;
        }
        
        public void ClearGoal(int index)
        {
            Goal goal = GetQuestInfo.goals[index];
            goal.isCompleted = true;
            GetQuestInfo.goals[index] = goal;

            if (IsClear)
            {
                EndQuest();
            }
        }
        
        public bool IsClearGoal(int index)
        {
            return GetQuestInfo.goals[index].isCompleted;
        }
    }
}
