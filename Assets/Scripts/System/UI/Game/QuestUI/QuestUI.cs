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
    private Color _selectedColor = new Color(0.7f, 0.7f, 0.7f);
    private Color _normalColor = Color.white;

    private Button _currentSelectedButton;

    protected override void Awake()
    {
        base.Awake();
        toggleKeyActionCode = ActionCode.Menu;
        SetQuestList();
    }

    public void SetQuestList()
    {
        foreach (Transform child in questListview.transform)
        {
            Destroy(child.gameObject);
        }

        var questComponent = PlayerCharacter.Instance.GetComponent<QuestComponent>();
        var questInfos = questComponent.GetQuestInfos();

        foreach (var questInfo in questInfos)
        {
            CreateQuestButton(questInfo);
        }

        _currentSelectedButton = null;
    }

    private void CreateQuestButton(SerializableQuestInfo questInfo)
    {
        var buttonObj = Instantiate(questPrefab, questListview.transform);
        var button = buttonObj.GetComponent<Button>();
        var buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
        buttonText.text = questInfo.QuestName;

        button.onClick.AddListener(() =>
        {
            if (_currentSelectedButton != null)
                _currentSelectedButton.image.color = _normalColor;
            
            button.image.color = _selectedColor;
            _currentSelectedButton = button;

            ShowQuestDetail(questInfo);
        });
        
        button.image.color = _normalColor;
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

