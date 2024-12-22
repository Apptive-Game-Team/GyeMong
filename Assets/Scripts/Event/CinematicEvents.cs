using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class CinematicEvent : Event { }

public class MoveCreatureEvent : CinematicEvent
{
    private enum CreatureType
    {
        Player,
        Selectable,
    }
    [SerializeField] private CreatureType creatureType;
    [SerializeField] private MonoBehaviour iControllable;
    [SerializeField] private Vector3 target;
    [SerializeField] private float speed;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        IControllable iControllable;
        if (creatureType == CreatureType.Selectable)
        {
            iControllable = (IControllable) this.iControllable;
        }
        else
        {
            iControllable = (IControllable) PlayerCharacter.Instance;
        }
        
        return iControllable.MoveTo(target, speed);
    }
}

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


public class ChangeSpriteEvent : CinematicEvent
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _sprite;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _spriteRenderer.sprite = _sprite;
        return null;
    }
}

public class ControlShaderEvent : CinematicEvent
{
    [SerializeField] private Shader shader;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        // 
        return null;
    }
}

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

public abstract class AnimationControllEvent : CinematicEvent
{
    [SerializeField] protected Animator _animator;
}

public class StopAnimatiorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        Debug.Log(_animator);
        _animator.enabled = false;
        return null;
    }
}

public class StartAnimatiorEvent : AnimationControllEvent
{
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        _animator.enabled = true;
        return null;
    }
}