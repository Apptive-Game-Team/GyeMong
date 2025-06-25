using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyCycleIndicatorUi : MonoBehaviour
{
    [SerializeField] private RectTransform niddle;
    private float currentAngle = 0f;
    public void UpdateClock(float timePercent)
    {
        float targetAngle = (timePercent * 360f) + 45f;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * 5f);
        niddle.rotation = Quaternion.Euler(0f, 0f, -currentAngle);
    }
}
