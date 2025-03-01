using System.Collections.Generic;
using UnityEngine;

namespace System.Game.Quest.Component
{
    [Serializable]
    public class QuestComponent : SingletonObject<QuestComponent>
    {
        [SerializeField] private List<Quests.Quest> _quests = new();

        public Quests.Quest GetQuest(int id)
        {
            return _quests.Find(quest => quest.GetQuestInfo.questID == id);
        }
        
        public void AddQuest(Quests.Quest quest)
        {
            _quests.Add(quest);
            quest.StartQuest();
        }

        public void SetQuests(List<Quests.Quest> quests)
        {
            _quests = quests;
        }

        public List<Quests.Quest> GetQuests()
        {
            return _quests;
        }
    }
}
