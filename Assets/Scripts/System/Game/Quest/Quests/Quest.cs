using System.Collections.Generic;

namespace System.Game.Quest.Quests
{
    public abstract class Quest
    {
        protected QuestInfo _questInfo;
        
        public String ClearConditionKey => _questInfo.questName + "IsCleared";
        public int NumOfGoals => _questInfo.goals.Count;
        public int NumOfCompletedGoals => _questInfo.goals.FindAll(goal => goal.isCompleted).Count;
        public int NumOfUncompletedGoals => _questInfo.goals.FindAll(goal => !goal.isCompleted).Count;
        public bool IsClear => NumOfGoals == NumOfCompletedGoals;

        public Quest(QuestInfo questInfo)
        {
            _questInfo = questInfo;
        }
        
        public abstract void StartQuest(); // Quest Start Event
        public abstract void UpdateQuest(); // Quest Update Event
        public abstract void EndQuest(); // Quest End Event
        
        public void ClearGoal(int index)
        {
            Goal goal = _questInfo.goals[index];
            goal.isCompleted = true;
            _questInfo.goals[index] = goal;
        }
        
        public bool IsClearGoal(int index)
        {
            return _questInfo.goals[index].isCompleted;
        }
        
        
        public enum QuestType
        {
            MAIN = 0,
            SUB = 1
        }
        public struct QuestInfo
        {
            public int questID;
            public string questName;
            public string questDescription;
            public QuestType questType;
            public List<Goal> goals;
        }

        public struct Goal // Quest's Goal
        {
            public string goalName;
            public string goalDescription;
            public bool isCompleted;
        }
    }
}
