using System.Collections;
using TMPro;
using UnityEngine;

public abstract class TextEvent : CinematicEvent
{
    protected const float DELTA_TIME = 0.02f;
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
    protected const float RANGE_TIME = 1f;
    [SerializeField] private string header;
    [SerializeField] private string description;
    [SerializeField] private Color textColor = Color.black;
    
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        HeaderText.text = header;
        HeaderText.color = textColor;
        DescriptionText.text = description;
        DescriptionText.color = textColor;
        Color color = HeaderText.color;
        color.a = 0;
        float timer = 0;
        while (timer < RANGE_TIME)
        {
            timer += DELTA_TIME;
            color.a += DELTA_TIME / RANGE_TIME;
            HeaderText.color = color;
            DescriptionText.color = color;
            yield return new WaitForSeconds(DELTA_TIME);
        }
        color.a = 1;
        HeaderText.color = color;
        DescriptionText.color = color;
        return null;
    }
}

public class ClearTextEvent : TextEvent
{
    protected const float RANGE_TIME = 0.5f;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Color color = HeaderText.color;
        color.a = 1;
        float timer = 0;
        while (timer < RANGE_TIME)
        {
            timer += DELTA_TIME; 
            color.a -= DELTA_TIME / RANGE_TIME;
            HeaderText.color = color;
            DescriptionText.color = color;
            yield return new WaitForSeconds(DELTA_TIME);
        }
        color.a = 0;
        HeaderText.color = color;
        DescriptionText.color = color;
        HeaderText.text = "";
        DescriptionText.text = "";
    }
}
