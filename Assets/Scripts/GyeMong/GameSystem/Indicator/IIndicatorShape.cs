using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public interface IIndicatorShape
    {
        GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot);
    }
}
