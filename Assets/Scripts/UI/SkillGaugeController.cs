using playerCharacter;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        if (SceneManager.GetActiveScene().name != "TitleScene") {
            base.UpdateSkillGauge();
            if (GetCurrentGauge() >= GetMaxGauge()) {
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
    }
}
