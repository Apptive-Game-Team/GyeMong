using System.Collections.Generic;
using System.Game.Quest.Quests;
using System.Linq;
using UnityEngine;
namespace System.Game.Quest.Component
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
