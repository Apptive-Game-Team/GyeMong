using System.Collections.Generic;
using playerCharacter;using UnityEngine;

public class SimplePlayerDistanceDetector : MonoBehaviour, IDetector<PlayerCharacter>
{
    private SimplePlayerDistanceDetector() { }

    public static SimplePlayerDistanceDetector Create(Creature.Creature creature)
    {
        SimplePlayerDistanceDetector detector = creature.gameObject.AddComponent<SimplePlayerDistanceDetector>();
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
        PlayerCharacter player = PlayerCharacter.Instance;
        
        if (Vector3.Distance(player.transform.position, transform.position) < creature.DetectionRange)
        {
            return player;
        }
        return null;
    }
}
