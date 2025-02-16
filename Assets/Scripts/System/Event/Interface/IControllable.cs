
using System.Collections;
using UnityEngine;

public interface IControllable
{
    IEnumerator MoveTo(Vector3 target, float speed);
}
