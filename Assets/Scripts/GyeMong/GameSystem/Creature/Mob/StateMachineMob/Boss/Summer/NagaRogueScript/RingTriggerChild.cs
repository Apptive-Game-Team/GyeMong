// 자식: 트리거 포워더(Outer/Inner에 각각 붙임)

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RingTriggerChild : MonoBehaviour
{
    public SandStormRing parent;
    public bool isInner; // Inner면 true, Outer면 false

    void OnTriggerEnter2D(Collider2D other) => parent.Mark(isInner, other, true);
    void OnTriggerExit2D(Collider2D other)  => parent.Mark(isInner, other, false);
}