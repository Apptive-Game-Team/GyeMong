using System.Collections;
using System.Event.Interface;
using playerCharacter;
using UnityEngine;

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
        bool isEnable = InputManager.Instance.GetKeyActive(ActionCode.MoveDown);
        InputManager.Instance.SetActionState(false);
        IControllable iControllable = null;
        if (creatureType == CreatureType.Selectable)
        {
            iControllable = (IControllable) this.iControllable;
        }
        else if (creatureType == CreatureType.Player)
        {
            iControllable = (IControllable) PlayerCharacter.Instance;
        }
        
        yield return iControllable.MoveTo(target, speed);
        InputManager.Instance.SetActionState(isEnable);
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

public class TimeScaleEvent : CinematicEvent
{
    [SerializeField] private float _timeScale = 0.1f;
    [SerializeField] private float _modifyDuration = 0.1f;


    public override IEnumerator Execute(EventObject eventObject = null)
    {
        float _nowTimeScale = Time.timeScale;
        Time.timeScale = _timeScale;
        yield return new WaitForSecondsRealtime(_modifyDuration);
        Time.timeScale = _nowTimeScale;
        
    }
}