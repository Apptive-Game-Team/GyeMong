using System;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    
    public enum MaterialType
    {
        DEFAULT,
        HIT,
        SHIELD,
    }
    
    
    [SerializeField] private MaterialDatas _materialDatas;
    private Renderer _renderer;
    private MaterialData _currentMaterialData;
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _currentMaterialData = _materialDatas.Get(MaterialType.DEFAULT);
        _renderer.material = _currentMaterialData.material;
    }
    
    public MaterialType GetCurrentMaterialType()
    {
        return _currentMaterialData.materialType;
    }
    
    public void SetMaterial(MaterialType type)
    {
        _currentMaterialData = _materialDatas.Get(type);
        _renderer.material = _currentMaterialData.material;
    }
    
    public void SetFloat(float value)
    {
        _renderer.material.SetFloat(_currentMaterialData.triggerName, value);
    }
    
    
    
}
