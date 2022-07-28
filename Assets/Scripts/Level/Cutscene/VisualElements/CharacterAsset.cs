using System.Collections.Generic;
using UnityEngine;

namespace LetterBattle
{
    [CreateAssetMenu(fileName = "Character_Asset", menuName = "Character_Asset", order = 0)]
    public class CharacterAsset : ScriptableObject
    {
        [SerializeField] private List<VisualElementAsset> elements;
        public IList<VisualElementAsset> Elements => elements;
    }
}