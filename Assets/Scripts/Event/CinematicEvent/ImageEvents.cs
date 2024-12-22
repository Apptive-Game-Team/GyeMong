using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ImageEvent : CinematicEvent
{
    private Image _image;
    protected Image Image
    {
        get
        {
            if (_image == null)
            {
                _image = EffectManager.Instance.transform.Find("EventImage").GetComponent<Image>();
            }
            return _image;
        }
    }
}

public class ShowImageEvent : ImageEvent
{
    [SerializeField] private Sprite sprite;
    
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Color color = Image.color;
        color.a = 1;
        Image.color = color;
        Image.sprite = sprite;
        return null;
    }
}
public class ClearImageEvent : ImageEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Color color = Image.color;
        color.a = 0;
        Image.color = color;
        Image.sprite = null;
        return null;
    }
}