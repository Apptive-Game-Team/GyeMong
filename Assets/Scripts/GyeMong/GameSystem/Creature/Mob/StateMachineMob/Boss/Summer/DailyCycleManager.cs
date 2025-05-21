using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyCycleManager : MonoBehaviour
{
    public enum TimeOfDay { Dawn, Day, Dusk, Night }

    public float secondsPerFullDay = 120f;
    public float currentTimePercent = 0f;

    public TimeOfDay currentSection;
    private float timePerSecond;

    void Start()
    {
        timePerSecond = 1f / secondsPerFullDay;
        UpdatePhase();
    }

    void Update()
    {
        currentTimePercent += timePerSecond * Time.deltaTime;
        if (currentTimePercent >= 1f) currentTimePercent = 0f;

        UpdatePhase();
    }

    void UpdatePhase()
    {
        TimeOfDay newPhase;

        if (currentTimePercent < 0.25f) newPhase = TimeOfDay.Dawn;
        else if (currentTimePercent < 0.5f) newPhase = TimeOfDay.Day;
        else if (currentTimePercent < 0.75f) newPhase = TimeOfDay.Dusk;
        else newPhase = TimeOfDay.Night;

        if (newPhase != currentSection)
        {
            currentSection = newPhase;
            OnPhaseChanged(currentSection);
        }
    }

    void OnPhaseChanged(TimeOfDay phase)
    {
    }
}
