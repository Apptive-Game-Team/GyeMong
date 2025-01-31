using System.Collections.Generic;
using playerCharacter;using UnityEngine;

public class SimplePlayerDetector : MonoBehaviour, IDetector<PlayerCharacter>
{
    private SimplePlayerDetector() { }

    public static SimplePlayerDetector Create(Creature creature)
    {
        SimplePlayerDetector detector = creature.gameObject.AddComponent<SimplePlayerDetector>();
        detector.creature = creature;
        return detector;
    }
    
    private Creature creature;
    
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
