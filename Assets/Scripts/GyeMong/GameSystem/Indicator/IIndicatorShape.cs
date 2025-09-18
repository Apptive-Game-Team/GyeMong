using UnityEngine;

namespace GyeMong.GameSystem.Indicator
{
    public interface IIndicatorShape
    {
        GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot);
        
        GameObject CreateIndicator(GameObject attackObject)
        {
            GameObject indicator = CreateIndicator(attackObject, attackObject.transform.position, attackObject.transform.rotation);
            return indicator;
        }
    }
}
