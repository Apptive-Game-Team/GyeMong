using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyemong.GameSystem.Rune.RuneUI.RuneTreeSystem
{
    public class CursorTargetSelector
    {
        List<SelectableUI> _targets;
        
        public CursorTargetSelector(List<SelectableUI> selectableUis)
        {   
            _targets = selectableUis;
        }

        public SelectableUI SelectTarget((int x, int y) direction, SelectableUI currentObject)
        {
            List<SelectableUI> candidates = SelectCandidates(direction, currentObject);
            return FindClosest(candidates, currentObject);
        }
        
        private List<SelectableUI> SelectCandidates((int x, int y) direction, SelectableUI currentObject)
        {
            float currentX;
            float currentY;
            if (currentObject == null)
            {
                currentX = 0;
                currentY = 0;
            } else {
                currentX = currentObject.transform.position.x;
                currentY = currentObject.transform.position.y;
            }
            List<SelectableUI> candidates = _targets.FindAll((target) =>
            {
                if (currentObject != target)
                {
                    float dx = target.transform.position.x - currentX;
                    float dy = target.transform.position.y - currentY;
                    return (direction.x == 0 || Mathf.Approximately(Mathf.Sign(dx), direction.x)) &&
                        (direction.y == 0 || Mathf.Approximately(Mathf.Sign(dy), direction.y)) ;
                }
                return false;
            });
            return candidates;
        }
        
        private SelectableUI FindClosest(List<SelectableUI> candidates, SelectableUI currentObject)
        {
            Vector3 currentPos;
            if (currentObject == null)
            {
                currentPos = Vector3.zero;
            }
            else
            {
                currentPos = currentObject.transform.position;
            }

            if (candidates.Count == 0)
            {
                return null;
            }
            SelectableUI closest = candidates
                .OrderBy(el => (el.transform.position - currentPos).sqrMagnitude).First();  
            return closest;
        }
    }
}