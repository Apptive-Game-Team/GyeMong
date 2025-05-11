using Gyemong.UISystem.Drawer;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Gyemong.GameSystem.Rune.RuneUI.RuneTreeSystem.LineDrawer
{
    public class CurveTreeLineDrawer : CurvesDrawer, ITreeLineDrawer
    {
        [SerializeField] private float amountOfCurves = 150;
        
        public void ClearLines()
        {
            Clear();
        }

        public void ConnectNodes(Vector2 position1, Vector2 position2)
        {
            AddCurve(new Vector2[]{position1, position1 + new Vector2(amountOfCurves, 0) , position2 - new Vector2(amountOfCurves, 0), position2});
        }
    }
}