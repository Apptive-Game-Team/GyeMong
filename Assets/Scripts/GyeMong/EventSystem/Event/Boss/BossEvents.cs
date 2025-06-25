using System.Collections;
using GyeMong.EventSystem.Controller;
using GyeMong.UISystem.Game.BossUI;
using UnityEngine;

namespace GyeMong.EventSystem.Event.Boss
{
  public abstract class BossEvent : Event
  {
    [SerializeField] protected GameSystem.Creature.Mob.StateMachineMob.Boss.Boss _boss;
  }
  public class ActivateBossRoomEvent : BossEvent
  {
    [SerializeField] protected GameObject bossRoomObject;
    public void SetBossRoomObject(GameObject obj)
    {
        bossRoomObject = obj;
    }
    public override IEnumerator Execute(EventObject eventObject = null)
    {
      bossRoomObject.SetActive(true);
      return null;
    }
  }
  public class DeActivateBossRoomEvent : BossEvent
  {
    [SerializeField] protected GameObject bossRoomObject;
    public void SetBossRoomObject(GameObject obj)
    {
        bossRoomObject = obj;
    }
        public override IEnumerator Execute(EventObject eventObject = null)
    {
      bossRoomObject.SetActive(false);
      return null;
    }
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

    public void SetBoss(GameSystem.Creature.Mob.StateMachineMob.Boss.Boss boss)
    {
        _boss = boss;
    }
        public override IEnumerator Execute(EventObject eventObject = null)
    {
      HpBarController.gameObject.SetActive(true);
      HpBarController.ClearBoss();
      HpBarController.UpdateHp(0,0);
    
      yield return DropHpBar();
      yield return ReboundHpBar();
      yield return FillHpBar();
    
      HpBarController.SetBoss(_boss);
    }

    private IEnumerator DropHpBar()
    {
      Vector3 defaultPosition = HpBarController.transform.position;
      Vector3 startPosition = defaultPosition + new Vector3(0, 100, 0);
      HpBarController.transform.position = startPosition;
      float timer = 0;
      while (timer < TIME)
      {
        timer += DELTA_TIME;
        float progress = Mathf.Pow(timer / TIME, 2);
        HpBarController.transform.position = Vector3.Lerp(startPosition, defaultPosition, progress);
        yield return new WaitForSeconds(DELTA_TIME);
      }
      HpBarController.transform.position = defaultPosition;
    }

    private IEnumerator ReboundHpBar()
    {
      Vector3 defaultPosition = HpBarController.transform.position;
      Vector3 reboundPosition = defaultPosition + new Vector3(0, 20, 0);
      float timer = 0;
      while (timer < TIME/2)
      {
        timer += DELTA_TIME;
        float progress = Mathf.Sin((timer / (TIME / 2)) * Mathf.PI);
        HpBarController.transform.position = Vector3.Lerp(defaultPosition, reboundPosition, progress);
        yield return new WaitForSeconds(DELTA_TIME);
      }
      HpBarController.transform.position = defaultPosition;
    }
  
    private IEnumerator FillHpBar()
    {
      HpBarController.UpdateHp(0,0);
      float timer = 0;
      float progress = 0;
      while (timer < FILL_TIME && progress <= 1)
      {
        timer += DELTA_TIME;
        HpBarController.UpdateHp(DEFAULT_HP * progress,0);
        progress = Mathf.Pow(timer / FILL_TIME, 2);
        yield return new WaitForSeconds(DELTA_TIME);
      }
      HpBarController.UpdateHp(DEFAULT_HP, 0);
    }
  }

  public class HideBossHealthBarEvent : BossHpBarEvent
  {
    public override IEnumerator Execute(EventObject eventObject = null)
    {
      if(HpBarController!=null)
      {
        HpBarController.ClearBoss();
        HpBarController.gameObject.SetActive(false);
      }
      return null;
    }
  }
}