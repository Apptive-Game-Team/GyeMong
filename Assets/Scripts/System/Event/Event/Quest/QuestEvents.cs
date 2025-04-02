using System.Collections;
using System.Game.Quest.Component;
using System.Game.Quest.Quests;
using playerCharacter;
using UnityEngine;
namespace System.Event.Event.Quest
{
    public abstract class QuestEvent : global::Event
    { }
    
    public class AddQuestEvent : QuestEvent
    {
        [SerializeField]
        private QuestInfo _questInfo;
        
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            Game.Quest.Quests.Quest quest = new Game.Quest.Quests.Quest(_questInfo);
            PlayerCharacter.Instance.GetComponent<QuestComponent>().AddQuest(quest);
            quest.StartQuest();
            return null;
        }
    }
    
    public class ClearQuestGoalEvent : QuestEvent
    {
        [SerializeField]
        private int _id;
       
        [ SerializeField]
        private int _goalIndex;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            Game.Quest.Quests.Quest quest = PlayerCharacter.Instance.GetComponent<QuestComponent>().GetQuest(_id);
            quest.ClearGoal(_goalIndex);
            return null;
        }
    }
}
