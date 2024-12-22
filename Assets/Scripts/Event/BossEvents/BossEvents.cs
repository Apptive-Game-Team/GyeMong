using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossEvent : Event
{
  [SerializeField] protected Boss _boss;
}

public abstract class BossHpBarEvent : BossEvent
{
  private HpBarController _hpBarController;

  protected HpBarController HpBarController
  {
    get
    {
      if (_hpBarController == null)
      {
        _hpBarController = EffectManager.Instance.GetHpBarController();
      }
      return _hpBarController;
    }
  }
}

public class ShowBossHealthBarEvent : BossHpBarEvent
{
  public override IEnumerator Execute(EventObject eventObject = null)
  {
    HpBarController.gameObject.SetActive(true);
    return null;
  }
}

public class HideBossHealthBarEvent : BossHpBarEvent
{
  public override IEnumerator Execute(EventObject eventObject = null)
  {
    HpBarController.gameObject.SetActive(false);
    return null;
  }
}