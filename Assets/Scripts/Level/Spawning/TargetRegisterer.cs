using System;
using UnityEngine;
namespace LetterBattle
{
    public class TargetRegisterer : MonoBehaviour
    {
        [SerializeField] private int order = 0;
        private void Start()
        {
            LevelManager.Current.RegisterAsTarget(this.transform,order);
        }
    }
}