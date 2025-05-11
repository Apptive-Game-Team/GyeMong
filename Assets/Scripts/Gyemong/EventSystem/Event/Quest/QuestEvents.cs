using System.Collections;
using Gyemong.GameSystem.Creature.Player;
using Gyemong.QuestSystem.Component;
using Gyemong.QuestSystem.Quests;
using Gyemong.UISystem.Game.QuestUI;
using UnityEngine;

namespace Gyemong.EventSystem.Event.Quest
{
    public abstract class QuestEvent : global::Gyemong.EventSystem.Event.Event
    { }
    
    public class AddQuestEvent : QuestEvent
    {
        [SerializeField]
        private QuestInfo _questInfo;
        
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            QuestSystem.Quests.Quest quest = new QuestSystem.Quests.Quest(_questInfo);
            PlayerCharacter.Instance.GetComponent<QuestComponent>().AddQuest(quest);
            quest.StartQuest();
            QuestUI.Instance.SetQuestList();
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
            QuestSystem.Quests.Quest quest = PlayerCharacter.Instance.GetComponent<QuestComponent>().GetQuest(_id);
            quest.ClearGoal(_goalIndex);
            return null;
        }
    }
}
