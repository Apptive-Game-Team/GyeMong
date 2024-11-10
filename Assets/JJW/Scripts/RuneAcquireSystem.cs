using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using runeSystem;

namespace runeSystem
{
    public class RuneAcquireSystem : MonoBehaviour
    {
        public RuneSystem runeSystem;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AcquireRune(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                AcquireRune(1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                AcquireRune(2);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                AcquireRune(3);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                AcquireRune(4);
            }
        }

        private void AcquireRune(int runeIndex)
        {
            if (runeSystem != null)
            {
                runeSystem.AcquireRune(runeIndex);

            }
        }
    }
}
