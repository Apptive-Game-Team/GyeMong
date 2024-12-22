using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
internal class Conditions
{
    private List<KeyValuePair<string, bool>> _conditions;

    public void Add(KeyValuePair<string, bool> condition)
    {
        _conditions.Add(condition);
    }

    public Conditions()
    {
        _conditions = new List<KeyValuePair<string, bool>>();
    }

    public Conditions(Dictionary<string, bool> conditions)
    {
        this._conditions = new List<KeyValuePair<string, bool>>();
        foreach (KeyValuePair<string, bool> condition in conditions)
        {
            this._conditions.Add(condition);
        }
    }

    public Dictionary<string, bool> GetConditions()
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();
        foreach (KeyValuePair<string, bool> condition in _conditions)
        {
            result.Add(condition.Key, condition.Value);
        }

        return result;
    }
}

public class ConditionManager : SingletonObject<ConditionManager>
{
    private const string CONDITION_FILE = "conditions";
    private Dictionary<string, bool> _conditions;
    public Dictionary<string, bool> Conditions => _conditions;
    protected override void Awake()
    {
        base.Awake();
        Conditions conditions = DataManager.Instance.LoadSection<Conditions>(CONDITION_FILE);
        _conditions = conditions.GetConditions();
    }
    
    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveSection(new Conditions(_conditions), CONDITION_FILE);
    }
}
