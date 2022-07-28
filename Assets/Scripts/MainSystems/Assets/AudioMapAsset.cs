using System.Collections.Generic;
using Cyberultimate;
using UnityEngine;
using LetterBattle.Utility;
namespace LetterBattle
{
    [CreateAssetMenu(menuName = "AudioMapAsset")]
    public class AudioMapAsset : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<SimpleDirection, AudioClip> map;
        public IReadOnlyDictionary<SimpleDirection, AudioClip> Map => map;
    }
    
}   