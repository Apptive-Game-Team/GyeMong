using System;
using UnityEngine;

[Serializable]
public class MaterialData
{
    public Material material;
    public MaterialController.MaterialType materialType;
    public string triggerName;
}

[CreateAssetMenu(fileName = "MaterialsData", menuName = "ScriptableObjects/MaterialsData")]
public class MaterialDatas : ScriptableObject
{
    public MaterialData[] materialDatas;

    public MaterialData Get(MaterialController.MaterialType type)
    {
        foreach (MaterialData materialData in materialDatas)
        {
            if (materialData.materialType == type)
            {
                return materialData;
            }
        }
        return null;
    }
}