using System.Collections.Generic;
using System.Linq;
using Gyemong.QuestSystem.Quests;
using UnityEngine;

namespace Gyemong.QuestSystem.Component
{
    
    public class QuestComponent : MonoBehaviour
    {
        private List<Quests.Quest> _quests = new();
        
        public Quests.Quest GetQuest(int id)
        {
            return _quests.Find(quest => quest.GetQuestInfo.questID == id);
        }
        
        public void AddQuest(Quests.Quest quest)
        {
            _quests.Add(quest);
            quest.StartQuest();
        }

        public void SetQuests(List<SerializableQuestInfo> quests)
        {
            _quests = quests.Select(questInfo => new Quests.Quest(questInfo)).ToList();
        }

        public List<SerializableQuestInfo> GetQuestInfos()
        {
            return _quests.Select(quest => (SerializableQuestInfo) quest.GetQuestInfo).ToList();
        }
    }
}
