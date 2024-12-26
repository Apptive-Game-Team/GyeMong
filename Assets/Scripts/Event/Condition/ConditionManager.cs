using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
internal class Conditions
{
    [Serializable] 
    class Condition
    {
        public string tag;
        public bool condition;
    }
    [SerializeReference] private List<Condition> _conditions;

    public void Add(string tag, string condition)
    {
        _conditions.Add(new Condition { tag = tag, condition = bool.Parse(condition) });
    }

    public Conditions()
    {
        _conditions = new List<Condition>();
    }

    public Conditions(Dictionary<string, bool> conditions)
    {
        this._conditions = new List<Condition>();
        if (conditions == null)
        {
            return;
        }
        foreach (KeyValuePair<string, bool> condition in conditions)
        {
            this._conditions.Add(new Condition{ tag = condition.Key, condition = condition.Value});
        }
    }

    public Dictionary<string, bool> GetConditions()
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();
        foreach (Condition condition in _conditions)
        {
            result.Add(condition.tag, condition.condition);
        }

        return result;
    }
}

public class ConditionManager : SingletonObject<ConditionManager>
{
    private const string CONDITION_FILE = "conditions";
    private Dictionary<string, bool> _conditions;
    public Dictionary<string, bool> Conditions
    {
        get
        {
            if (_conditions == null)
            {
                _conditions = DataManager.Instance.LoadSection<Conditions>(CONDITION_FILE).GetConditions();
            }

            return _conditions;
        }
    }
    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveSection(new Conditions(_conditions), CONDITION_FILE);
    }

    public void Save()
    {
        DataManager.Instance.SaveSection(new Conditions(_conditions), CONDITION_FILE);
    }
}
