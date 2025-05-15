using System.Collections;
using UnityEngine;

namespace GyeMong.EventSystem.Interface
{
    public interface IControllable
    {
        IEnumerator MoveTo(Vector3 target, float speed);
    }
}
