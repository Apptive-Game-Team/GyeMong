using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Quest.Component
{
    public class QuestComponent : SingletonObject<QuestComponent>
    {
        [SerializeField] private List<Quests.Quest> _closedQuests = new();
        [SerializeField] private List<Quests.Quest> _activeQuests = new();

        public Quests.Quest GetQuest(int id)
        {
            return _activeQuests.Find(quest => quest.GetQuestInfo.questID == id);
        }
        
        public void AddQuest(Quests.Quest quest)
        {
            _activeQuests.Add(quest);
            quest.StartQuest();
        }
        
        public void UpdateQuests()
        {
            CheckClear();
        }

        private void CheckClear()
        {
            foreach (Quests.Quest quest in _activeQuests)
            {
                if (quest.IsClear)
                {
                    quest.EndQuest();
                    _closedQuests.Add(quest);
                    _activeQuests.Remove(quest);
                }
            }
        }
    }
}
