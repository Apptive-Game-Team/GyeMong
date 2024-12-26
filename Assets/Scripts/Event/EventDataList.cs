using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventData
{
    public string name;
    public int id;
    public EventObject eventObj;
}

[CreateAssetMenu(fileName = "EventDataList",menuName = "ScriptableObject/EventDataList")]
public class EventDataList : ScriptableObject
{
    public List<EventData> eventDataList;

    public EventObject GetEvent(int id)
    {
        EventObject @event = eventDataList.Find(x => x.id == id).eventObj;
        if (@event == null)
        {
            Debug.LogError("There's No Event that has this id!");
            return null;
        }

        return @event;
    }
    public EventObject GetEvent(string name)
    {
        EventObject @event = eventDataList.Find(x => x.name == name).eventObj;
        if (@event == null)
        {
            Debug.LogError("There's No Event that has this name!");
            return null;
        }

        return @event;
    }
}
