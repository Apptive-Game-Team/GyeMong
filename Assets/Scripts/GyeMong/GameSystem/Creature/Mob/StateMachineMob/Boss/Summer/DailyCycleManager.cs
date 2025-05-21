using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DailyCycleManager : MonoBehaviour
{
    public enum TimeOfDay { Dawn, Day, Dusk, Night }

    public float secondsPerFullDay = 120f;
    public float currentTimePercent;
    public Light2D globalLight2D;

    private float timePerSecond;

    private Color dawnColor = new Color32(145, 145, 145, 255);
    private Color dayColor = new Color32(255, 255, 255, 255);
    private Color duskColor = new Color32(145, 145, 145, 255);
    private Color nightColor = new Color32(30, 30, 30, 255);

    public TimeOfDay currentTime;

    private void Start()
    {
        currentTimePercent = 0.75f;
        timePerSecond = 1f / secondsPerFullDay;
    }

    private void Update()
    {
        currentTimePercent += timePerSecond * Time.deltaTime;
        if (currentTimePercent >= 1f)
            currentTimePercent -= 1f;

        UpdateLighting();
        UpdateTime();
    }
    void UpdateTime()
    {
        TimeOfDay newPhase;

        if (currentTimePercent < 0.25f) newPhase = TimeOfDay.Dawn;
        else if (currentTimePercent < 0.5f) newPhase = TimeOfDay.Day;
        else if (currentTimePercent < 0.75f) newPhase = TimeOfDay.Dusk;
        else newPhase = TimeOfDay.Night;

        if (newPhase != currentTime)
        {
            currentTime = newPhase;
        }
    }
    void UpdateLighting()
    {
        Color targetColor;

        if (currentTimePercent < 0.25f)
        {
            float t = currentTimePercent / 0.25f;
            targetColor = Color.Lerp(dawnColor, dayColor, t);
        }
        else if (currentTimePercent < 0.5f)
        {
            float t = (currentTimePercent - 0.25f) / 0.25f;
            targetColor = Color.Lerp(dayColor, duskColor, t);
        }
        else if (currentTimePercent < 0.75f)
        {
            float t = (currentTimePercent - 0.5f) / 0.25f;
            targetColor = Color.Lerp(duskColor, nightColor, t);
        }
        else
        {
            float t = (currentTimePercent - 0.75f) / 0.25f;
            targetColor = Color.Lerp(nightColor, dawnColor, t);
        }

        globalLight2D.color = targetColor;
    }
}

