using GyeMong.GameSystem.Map.Portal;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.ScriptableObject
{
    [CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObjects/StageInfo", order = 1)]
    public class StageInfo : UnityEngine.ScriptableObject {
        public Select.Stage id;
        public PortalID portalID;
        public Script beforeScript;
        public Script afterScript;
    }
}