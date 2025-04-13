using System;
using System.Collections;
using System.Collections.Generic;
using System.Event.Interface;
using System.Input;
using Map.Puzzle.Maze;
using playerCharacter;
using UnityEngine;

public abstract class CinematicEvent : Event { }

public class ChangeDarkness : CinematicEvent
{
    [SerializeField] private DarknessController _darknessController;
    [SerializeField] private float _intensity;
    [SerializeField] private float _duration;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        return _darknessController.ChangeIntensity(_intensity, _duration);
    }
}

public class MoveCreatureEvent : CinematicEvent
{
    public enum CreatureType
    {
        Player,
        Selectable,
    }
    [SerializeField] public  CreatureType creatureType;
    [SerializeField] public  MonoBehaviour iControllable;
    [SerializeField] public  Vector3 target;
    [SerializeField] public  float speed;
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
        else
        {
            throw new Exception("Invalid CreatureType");
        }
        
        yield return iControllable.MoveTo(target, speed);
        InputManager.Instance.SetActionState(isEnable);
    }
}

public class MoveCreatureByGameObjectEvent : CinematicEvent
{
    [SerializeField] private MoveCreatureEvent.CreatureType _creatureType;
    [SerializeField] private MonoBehaviour _iControllable;
    [SerializeField] private List<Target> _targets;
    [Serializable]
    public struct Target
    {
        public Transform transform;
        public float speed;
    }
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        MoveCreatureEvent moveCreatureEvent = new MoveCreatureEvent();
        moveCreatureEvent.creatureType = _creatureType;
        moveCreatureEvent.iControllable = _iControllable;
        
        foreach (var target in _targets)
        {
            moveCreatureEvent.target = target.transform.position;
            moveCreatureEvent.speed = target.speed;
            yield return moveCreatureEvent.Execute();
        }
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