using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerEventType
{
    None = 0,
    OnTakeDamage = 1,
    OnAttackEnemy = 2,
}

public class PlayerObservingBuff
{
    public PlayerEventType EventType;
    private BuffData _buffData;
    
    public PlayerObservingBuff(PlayerEventType eventType, BuffData buffData)
    {
        this.EventType = eventType;
        _buffData = buffData;
    }

    public void OnNotified()
    {
        Debug.Log("PlayerObservingBuff OnNotified");
    }
}

public class PlayerEventObserver : MonoBehaviour
{
    private List<PlayerObservingBuff> _observingBuffList;
    private List<PlayerObservingBuff> _notifyBuffList;
    
    public void AddObserver(PlayerEventType eventType, BuffData buffData)
    {
        _observingBuffList.Add(new PlayerObservingBuff(eventType, buffData));
    }

    public void RemoveObserver(PlayerObservingBuff observingBuff)
    {
        _observingBuffList.Remove(observingBuff);
    }

    public void NotifyEvent(PlayerEventType eventType)
    {
        _notifyBuffList.Clear();
        _notifyBuffList = _observingBuffList.FindAll(x => x.EventType.Equals(eventType));
        foreach (var buff in _notifyBuffList)
        {
            buff.OnNotified();
        }
    }
}
