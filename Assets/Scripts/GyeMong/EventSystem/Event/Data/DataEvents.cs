// using System.Collections;
// using GyeMong.DataSystem;
// using GyeMong.EventSystem.Controller.Condition;
// using GyeMong.GameSystem.Creature.Player;
// using UnityEngine.SceneManagement;
//
// namespace GyeMong.EventSystem.Event.Data
// {
//     public abstract class DataEvent : Event
//     {
//         public override IEnumerator Execute(EventObject eventObject = null)
//         {
//             yield return null;
//         }
//     }
//     public class DataEvents : DataEvent
//     {
//         private PlayerData playerData = new();
//         public override IEnumerator Execute(EventObject eventObject = null)
//         {
//             playerData.isFirst = false;
//             playerData.sceneName = SceneManager.GetActiveScene().name;
//             
//             DataManager.Instance.SaveSection(playerData, "PlayerData");
//             ConditionManager.Instance.Save();
//
//             yield return null;
//         }
//     }
//     public class LoadDataEvent : DataEvent
//     {
//         private PlayerData playerData;
//         public override IEnumerator Execute(EventObject eventObject = null)
//         {
//             playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
//             SceneManager.LoadScene(playerData.sceneName);
//             DataManager.Instance.LoadPlayerData();
//             ConditionManager.Instance.Load();
//             yield return null;
//         }
//     }
// }