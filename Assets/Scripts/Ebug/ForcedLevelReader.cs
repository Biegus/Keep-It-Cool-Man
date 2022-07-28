

using System;
using UnityEngine;
namespace LetterBattle.Utility
{
    public class ForcedLevelReader : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            DebugLevelRunner.ReadAndStart(true);
#endif
        }
    }
}

