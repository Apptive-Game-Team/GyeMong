using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class SkillGaugeController : MonoBehaviour
{
    private Slider skillSlider;

    private void Awake()
    {
        skillSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        UpdateSkillGauge();
    }

    private void UpdateSkillGauge()
    {
        skillSlider.value = PlayerCharacter.Instance.GetCurSkillGauge() / PlayerCharacter.Instance.maxSkillGauge;
    }
}
