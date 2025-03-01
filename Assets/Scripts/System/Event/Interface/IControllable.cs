using System.Collections;
using UnityEngine;

namespace System.Event.Interface
{
    public interface IControllable
    {
        IEnumerator MoveTo(Vector3 target, float speed);
    }
}
