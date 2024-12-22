using System.Collections;
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
        IControllable iControllable = null;
        if (creatureType == CreatureType.Selectable)
        {
            iControllable = (IControllable) this.iControllable;
        }
        else if (creatureType == CreatureType.Player)
        {
            iControllable = (IControllable) PlayerCharacter.Instance;
        }
        
        return iControllable.MoveTo(target, speed);
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