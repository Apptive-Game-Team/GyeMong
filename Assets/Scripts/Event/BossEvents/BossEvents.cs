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
  private const float TIME = 0.3f;
  private const float DELTA_TIME = 0.02f;
  public override IEnumerator Execute(EventObject eventObject = null)
  {
    HpBarController.gameObject.SetActive(true);
    HpBarController.SetBoss(_boss);

    Vector3 defaultScale = HpBarController.transform.localScale;
    HpBarController.transform.localScale = defaultScale * 0.8f;
    float timer = 0;

    while (timer < TIME)
    {
      timer += DELTA_TIME;
      float progress = timer / TIME;
      float scaleRate = Mathf.Lerp(0.8f, 1, Mathf.Sin(progress * Mathf.PI));
      HpBarController.transform.localScale = defaultScale * scaleRate;

      yield return new WaitForSeconds(DELTA_TIME);
    }

    HpBarController.transform.localScale = defaultScale;
  }
}

public class HideBossHealthBarEvent : BossHpBarEvent
{
  public override IEnumerator Execute(EventObject eventObject = null)
  {
    HpBarController.ClearBoss();
    HpBarController.gameObject.SetActive(false);
    return null;
  }
}