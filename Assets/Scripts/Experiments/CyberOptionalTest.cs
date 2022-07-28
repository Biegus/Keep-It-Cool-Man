using System;
using LetterBattle;
using UnityEngine;
namespace Experiments
{
    public class CyberOptionalTest : MonoBehaviour
    {
        [SerializeField] private CyberOptional<int> optional;
        [SerializeField] private int normalInt;
        [NaughtyAttributes.Button()]
        public void Print()
        {
            print(optional.GetValue(-1));
        }
    }
}