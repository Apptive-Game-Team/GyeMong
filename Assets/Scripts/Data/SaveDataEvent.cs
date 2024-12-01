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

        yield return null;
    }
}
