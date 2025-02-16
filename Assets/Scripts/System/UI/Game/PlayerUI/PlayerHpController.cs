using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public class PlayerHpController : GaugeController
{
    protected override float GetCurrentGauge()
    {
        return PlayerCharacter.Instance.CurrentHp;
    }

    protected override float GetMaxGauge()
    {
        return PlayerCharacter.Instance.MaxHp;
    }
}
