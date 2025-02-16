using System.Collections;
using playerCharacter;
using UnityEngine.SceneManagement;
public class SaveDataEvent : Event
{
    private PlayerData playerData = new();
    private RuneDatas runeData = new();
    private RuneComponent runeComponent;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData.isFirst = false;
        playerData.sceneName = SceneManager.GetActiveScene().name;
        playerData.playerPosition = PlayerCharacter.Instance.GetPlayerPosition();
        playerData.playerDirection = PlayerCharacter.Instance.GetPlayerDirection();

        runeComponent = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        runeData.AcquiredRuneDatas = runeComponent.AcquiredRuneList;
        runeData.EquippedRuneDatas = runeComponent.EquippedRuneList;

        DataManager.Instance.SaveSection(playerData, "PlayerData");
        DataManager.Instance.SaveSection(runeData, "RuneData");
        // ConditionManager.Instance.Conditions.Add("spring_puzzle3_clear", puzzle3Flag.puzzle3Flag);
        // ConditionManager.Instance.Conditions["spring_puzzle3_clear"] = puzzle3Flag.puzzle3Flag;
        // ConditionManager.Instance.Conditions["spring_puzzle3_clear"]
        ConditionManager.Instance.Save();

        yield return null;
    }
}
public class LoadDataEvent : Event
{
    private PlayerData playerData;
    private RuneDatas runeData;
    private RuneComponent runeComponent;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
        runeData = DataManager.Instance.LoadSection<RuneDatas>("RuneData");

        runeComponent = PlayerCharacter.Instance.GetComponent<RuneComponent>();

        for (int i = 0;i < runeData.AcquiredRuneDatas.Count;i++) 
        {
            runeComponent.AcquireRune(runeData.AcquiredRuneDatas[i]);
        }
        for (int i = 0;i < runeData.EquippedRuneDatas.Count;i++)
        {
            runeComponent.EquipRune(runeData.EquippedRuneDatas[i]);
        }

        SceneManager.LoadScene(playerData.sceneName);
        DataManager.Instance.LoadPlayerData();
        yield return null;
    }
}


