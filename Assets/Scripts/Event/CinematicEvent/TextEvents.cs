using System.Collections;
using TMPro;
using UnityEngine;

public abstract class TextEvent : CinematicEvent
{
    private GameObject _textPanel;
    protected GameObject TextPanel
    {
        get
        {
            if (_textPanel == null)
            {
                _textPanel = EffectManager.Instance.transform.Find("EventText").gameObject;
            }
            return _textPanel;
        }
    }

    private TMP_Text _headerText;
    protected TMP_Text HeaderText
    {
        get
        {
            if (_headerText == null)
            {
                _headerText = TextPanel.transform.Find("Header").GetComponent<TMP_Text>();
            }
            return _headerText;
        }
    }
    private TMP_Text _descriptionText;
    protected TMP_Text DescriptionText
    {
        get
        {
            if (_descriptionText == null)
            {
                _descriptionText = TextPanel.transform.Find("Description").GetComponent<TMP_Text>();
            }
            return _descriptionText;
        }
    }
}

public class PrintTextEvent : TextEvent
{
    [SerializeField] private string header;
    [SerializeField] private string description;
    
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        HeaderText.text = header;
        DescriptionText.text = description;
        return null;
    }
}

public class ClearTextEvent : TextEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        HeaderText.text = "";
        DescriptionText.text = "";
        return null;
    }
}