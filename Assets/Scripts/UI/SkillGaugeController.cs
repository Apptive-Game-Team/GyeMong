using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class SkillGaugeController : MonoBehaviour
{
    private Slider skillSlider;
    private Material gaugeEffectMaterial;

    private void Awake()
    {
        skillSlider = GetComponent<Slider>();
        gaugeEffectMaterial = transform.Find("Background").GetComponent<Image>().material;
    }

    private void Update()
    {
        UpdateSkillGauge();
    }

    private void UpdateSkillGauge()
    {
        skillSlider.value = PlayerCharacter.Instance.GetCurSkillGauge() / PlayerCharacter.Instance.maxSkillGauge;
        if (PlayerCharacter.Instance.GetCurSkillGauge() >= PlayerCharacter.Instance.skillUsageGauge)
        {
            gaugeEffectMaterial.SetFloat("_isUsable", 1);
        }
        else
        {
            gaugeEffectMaterial.SetFloat("_isUsable", 0);
        }
    }
}
