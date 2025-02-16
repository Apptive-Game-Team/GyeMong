using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class GaugeController : MonoBehaviour
{
    private Slider skillSlider;

    protected abstract float GetCurrentGauge();
    protected abstract float GetMaxGauge();
    
    protected virtual void Awake()
    {
        skillSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        UpdateSkillGauge();
    }

    protected virtual void UpdateSkillGauge()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene") {
            return;
        }
        skillSlider.value = GetCurrentGauge() / GetMaxGauge();
    }
}
