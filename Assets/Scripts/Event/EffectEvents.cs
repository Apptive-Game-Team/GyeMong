using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : Event
{

    public override IEnumerator execute(EventObject eventObject = null)
    {
        ParticleSystem particleSystem = eventObject.GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        particleSystem.Stop();
    }
}
