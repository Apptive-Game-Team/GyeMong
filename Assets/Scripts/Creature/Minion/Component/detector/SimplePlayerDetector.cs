using System.Collections.Generic;
using playerCharacter;using UnityEngine;

public class SimplePlayerDetector : MonoBehaviour, IDetector<PlayerCharacter>
{
    private SimplePlayerDetector() { }

    public static SimplePlayerDetector Create(Creature.Creature creature)
    {
        SimplePlayerDetector detector = creature.gameObject.AddComponent<SimplePlayerDetector>();
        detector.creature = creature;
        return detector;
    }
    
    private Creature.Creature creature;
    
    public List<PlayerCharacter> DetectTargets()
    {
        return new List<PlayerCharacter> { DetectTarget() };
    }

    public PlayerCharacter DetectTarget()
    {
        return PlayerCharacter.Instance;
    }
}
