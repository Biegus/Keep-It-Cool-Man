using System;
using LetterBattle;
using UnityEngine;
using SerializedType = UnityEngine.SerializedType;
namespace LetterBattle
{
    [CreateAssetMenu(fileName = "LetterType", menuName = "LetterType", order = 0)]
    public class LetterType : ScriptableObject
    {
        [ClassExtends(typeof(SpawnBehavior), Grouping = ClassGrouping.None, AddTextSearchField = true)]
        [SerializeField]
        private SerializedType behaviourType;
        [PrefabObjectOnly]
        [SerializeField,NotNull]
        private GameObject prefab;
        
        public SerializedType BehaviourType => behaviourType;
        public GameObject Prefab => prefab;
    }
    
    #if UNITY_EDITOR
    #endif
}