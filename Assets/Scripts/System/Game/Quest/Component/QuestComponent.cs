using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Quest.Component
{
    public class QuestComponent : SingletonObject<QuestComponent>
    {
        private List<Quests.Quest> _closedQuests = new();
        private List<Quests.Quest> _activeQuests = new();
        
        public void AddQuest(Quests.Quest quest)
        {
            _activeQuests.Add(quest);
            quest.StartQuest();
        }
        
        public void UpdateQuests()
        {
            CheckClear();
            foreach (Quests.Quest quest in _activeQuests)
            {
                quest.UpdateQuest();
            }
        }

        private void CheckClear()
        {
            foreach (Quests.Quest quest in _activeQuests)
            {
                if (quest.IsClear())
                {
                    quest.EndQuest();
                    _closedQuests.Add(quest);
                    _activeQuests.Remove(quest);
                }
            }
        }
    }
}
