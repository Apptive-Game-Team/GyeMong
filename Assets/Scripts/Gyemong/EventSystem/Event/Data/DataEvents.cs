using System.Collections;
using Gyemong.DataSystem;
using Gyemong.EventSystem.Controller.Condition;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine.SceneManagement;

namespace Gyemong.EventSystem.Event.Data
{
    public abstract class DataEvent : Event
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            yield return null;
        }
    }
    public class DataEvents : DataEvent
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
    public class LoadDataEvent : DataEvent
    {
        private PlayerData playerData;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
            SceneManager.LoadScene(playerData.sceneName);
            DataManager.Instance.LoadPlayerData();
            ConditionManager.Instance.Load();
            yield return null;
        }
    }
}