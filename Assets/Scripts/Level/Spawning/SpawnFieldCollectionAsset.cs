using System.Collections.Generic;
using UnityEngine;
namespace LetterBattle
{
    [CreateAssetMenu(fileName = "SpawnFieldAsset", menuName = "SpawnFieldAsset", order = 0)]
    public class SpawnFieldCollectionAsset : ScriptableObject
    {
        [SerializeField] private SpawnField[] fields;
        public IReadOnlyCollection<SpawnField> Fields => fields;
    }
}