using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTestComponent : MonoBehaviour
{
    [SerializeField] float playerHealth = 100f;
    [SerializeField] BuffComponent buffComponent;

    private void Awake()
    {
        buffComponent = new BuffComponent();
    }

    private void Start()
    {
        buffComponent.AddBuff(new BuffData() { buffType = BuffType.SNARE, disposeMode = BuffDisposeMode.TEMPORARY, duration = 5f});
    }
}
