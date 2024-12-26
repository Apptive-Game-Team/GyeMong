using System.Collections;
using playerCharacter;
using UnityEngine.SceneManagement;
public class SaveDataEvent : Event
{
    private PlayerData playerData = new();
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData.isFirst = false;
        playerData.sceneName = SceneManager.GetActiveScene().name;
        playerData.playerPosition = PlayerCharacter.Instance.GetPlayerPosition();
        playerData.playerDirection = PlayerCharacter.Instance.GetPlayerDirection();

        DataManager.Instance.SaveSection(playerData, "PlayerData");
        ConditionManager.Instance.Save();

        yield return null;
    }
}
public class LoadDataEvent : Event
{
    private PlayerData playerData;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");

        SceneManager.LoadScene(playerData.sceneName);
        PlayerCharacter.Instance.LoadPlayerData();
        yield return null;
    }
}


