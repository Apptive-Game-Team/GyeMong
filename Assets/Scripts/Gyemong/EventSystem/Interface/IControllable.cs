using System.Collections;
using UnityEngine;

namespace Gyemong.EventSystem.Interface
{
    public interface IControllable
    {
        IEnumerator MoveTo(Vector3 target, float speed);
    }
}
