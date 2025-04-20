using System.Input;
using UnityEngine;
using playerCharacter;
using Util.QuestSystem.Component;
using Util.QuestSystem.Quests;
using TMPro;
using UnityEngine.UI;

namespace System.UI.Game.QuestUI
{
    public class QuestUI : UIWindowToggler<QuestUI>
    { 
        [SerializeField] private GameObject questListview;
        [SerializeField] private GameObject questPrefab;
        [SerializeField] private TMP_Text questDescriptionText;
        [SerializeField] private TMP_Text goalDescriptionText;
    
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.Menu;
            SetQuestList();
        }
        
        public void SetQuestList()
        {
            var questComponent = PlayerCharacter.Instance.GetComponent<QuestComponent>();
            var questInfos = questComponent.GetQuestInfos();
            
            foreach (Transform child in questListview.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var questInfo in questInfos)
            {
                CreateQuestButton(questInfo);
            }
        }
        
        private void CreateQuestButton(SerializableQuestInfo questInfo)
        {
            var buttonObj = Instantiate(questPrefab, questListview.transform);
            buttonObj.GetComponentInChildren<TMP_Text>().text = questInfo.QuestName;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowQuestDetail(questInfo);
            });
        }
        
        private void ShowQuestDetail(SerializableQuestInfo questInfo)
        {
            questDescriptionText.text = questInfo.QuestDescription;
            
            var goalsText = "";
            foreach (var goal in questInfo.Goals)
            {
                goalsText += $"- {goal.goalDescription}\n";
            }

            goalDescriptionText.text = goalsText;
        }
    }
}

