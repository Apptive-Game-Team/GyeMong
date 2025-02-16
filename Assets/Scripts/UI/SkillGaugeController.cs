using playerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class SkillGaugeController : GaugeController
{
    private Material gaugeEffectMaterial;

    protected override float GetCurrentGauge()
    {
        return PlayerCharacter.Instance.GetCurSkillGauge();
    }

    protected override float GetMaxGauge()
    {
        return PlayerCharacter.Instance.maxSkillGauge;
    }

    protected override void Awake()
    {
        base.Awake();
        gaugeEffectMaterial = transform.Find("Background").GetComponent<Image>().material;
    }
    

    private void UpdateSkillGauge()
    {
        base.UpdateSkillGauge();
        if (GetCurrentGauge() >= GetMaxGauge())
        {
            gaugeEffectMaterial.SetFloat("_isUsable", 1);
        }
        else
        {
            gaugeEffectMaterial.SetFloat("_isUsable", 0);
        }
    }
}
