using System;
using UnityEngine;
namespace LetterBattle.Utility
{
    public class SharedTemplateTest : MonoBehaviour
    {
        [Serializable]
        public struct TestStruct
        {
            public int x;
            public int y;
        }
        [UseSharedTemplate("Test/Test")]
        public int value;

        [UseSharedTemplate("Test/Test2")]
        public float floatValue;
      

        [UseSharedTemplate("Test/DmgSound")]
        public UnityEngine.AudioClip dmgSound;
        [UseSharedTemplate("Test/OtherSound")]
        public UnityEngine.AudioClip otherSound;
    }
}