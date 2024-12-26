using System.Collections;
using System.Collections.Generic;
using System.Data;
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
  private const float FILL_TIME = 0.7f;
  private const float DELTA_TIME = 0.02f;
  private const float DEFAULT_HP = 100;
  public override IEnumerator Execute(EventObject eventObject = null)
  {
    HpBarController.gameObject.SetActive(true);
    HpBarController.ClearBoss();
    HpBarController.UpdateHp(0);

    // Vector3 defaultScale = HpBarController.transform.localScale;
    Vector3 defaultPosition = HpBarController.transform.position;

    // 시작 위치 설정 (화면 위에서 시작)
    Vector3 startPosition = defaultPosition + new Vector3(0, 100, 0); // Y축으로 2.0만큼 위로 이동
    
    HpBarController.transform.position = startPosition;
    // HpBarController.transform.localScale = defaultScale * 0.9f;
    float timer = 0;

    while (timer < TIME)
    {
      timer += DELTA_TIME;
      float progress = Mathf.Pow(timer / TIME, 2);
      float scaleRate = Mathf.Lerp(0.9f, 1, Mathf.Sin(progress * Mathf.PI));
      // HpBarController.transform.localScale = defaultScale * scaleRate;
      HpBarController.transform.position = Vector3.Lerp(startPosition, defaultPosition, progress );

      yield return new WaitForSeconds(DELTA_TIME);
    }
    HpBarController.transform.position = defaultPosition;

    // HpBarController.transform.localScale = defaultScale;

    yield return FillHpBar();
    HpBarController.SetBoss(_boss);
  }

  private IEnumerator FillHpBar()
  {
    HpBarController.UpdateHp(0);
    float timer = 0;
    float progress = 0;
    while (timer < FILL_TIME && progress <= 1)
    {
      timer += DELTA_TIME;
      HpBarController.UpdateHp(DEFAULT_HP * progress);
      progress = Mathf.Pow(timer / FILL_TIME, 2);
      yield return new WaitForSeconds(DELTA_TIME);
    }
    HpBarController.UpdateHp(DEFAULT_HP);
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