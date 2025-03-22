using System.Collections;
using System.Collections.Generic;
using System.Event;
using Creature.Player;
using UnityEngine;

namespace Map
{
    public class MapSectionDetector : MonoBehaviour
    {
        [SerializeField]
        private List<Vector2> polygon = null;
        private EventObject eventObject;
        bool IsPointInsidePolygon(Vector3 point, List<Vector2> polygon)
        {
            int intersectCount = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 p1 = polygon[i];
                Vector2 p2 = polygon[(i + 1) % polygon.Count];

                if ((point.y > Mathf.Min(p1.y, p2.y) && point.y <= Mathf.Max(p1.y, p2.y)) &&
                    (point.x <= Mathf.Max(p1.x, p2.x)))
                {
                    float xIntersect = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                    if (p1.y != p2.y && point.x <= xIntersect)
                    {
                        intersectCount++;
                    }
                }
            }
            return (intersectCount % 2) != 0;
        }

        private void Start()
        {
            eventObject = GetComponent<EventObject>();
            if (polygon.Count == 0)
            {
                eventObject.Trigger();
            }
            else
            {
                StartCoroutine(CheckingMapSection());
            }
        }

        [SerializeField]
        private bool wasIn = false;
    
        private IEnumerator CheckingMapSection()
        {
            while (true)
            {
                bool isIn = IsPointInsidePolygon(PlayerCharacter.Instance.transform.position, polygon);
                if (isIn)
                {
                    if (!wasIn)
                    {
                        eventObject.Trigger();
                    }
                    wasIn = true;
                } else if (!isIn)
                {
                    wasIn = false;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}