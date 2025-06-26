using UnityEngine;
using UnityEngine.UI;

public class BodyHeatUI : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;

    public void SetHeat(float value)
    {
        gaugeImage.fillAmount = Mathf.Clamp01(value);
    }
    public void SetColorByHeat(Color coldColor, Color hotColor, float value)
    {
        gaugeImage.color = Color.Lerp(coldColor, hotColor, Mathf.Clamp01(value));
    }
}