using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Cyberultimate.Unity
{
    [CreateAssetMenu(menuName = "ColorBlockAsset", fileName ="color block")]
    public class ButtonTransitionAsset : ScriptableObject
    {
        [FormerlySerializedAs("colors")] [SerializeField] private ColorBlock content;
        public ColorBlock Content => content;
        public int RefreshCount { get; private set; }
        private void OnValidate()
        {
            RefreshCount++;
            
        }


    }
    
}