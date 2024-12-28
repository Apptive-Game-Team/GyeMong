using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCreator : SingletonObject<EffectCreator>
{
    [SerializeField] EffectDataList effectDataList;

    [SerializeField] List<EffectObject> effectPool;
    [SerializeField] int poolSize = 10;

    public void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            EffectObject effect = new EffectObject();
            effectPool.Add(effect);
        }
    }

    public EffectObject GetPool(int id, Vector3 pos)
    {
        EffectObject eff = effectPool.Find(x => x.ID.Equals(id));
        if (eff = null)
        {
            Debug.LogError("There's no EffectObj with this id!");
            return null;
        }
        eff.transform.position = pos;
        eff.gameObject.SetActive(true);
        return eff;
    }

    public void CreateEffect(int id, Vector3 pos)
    {
        Instantiate(effectDataList.GetEffectData(id).effectPrefab, pos, Quaternion.identity);
    }

    public void CreateEffect(int id, Transform tr)
    {
        Instantiate(effectDataList.GetEffectData(id).effectPrefab, tr);
    }
}
