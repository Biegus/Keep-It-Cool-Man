using LetterBattle.Utility;
using UnityEngine;

namespace LetterBattle
{
    [CreateAssetMenu(fileName = "_bg_type", menuName = "BackgroundType", order = 0)]
    public class BackgroundTypeAsset : ScriptableObject
    {
        [NaughtyAttributes.MinMaxSlider(1, 60)]
        [Tooltip("Delay in seconds between debry spawn")]
        [SerializeField]
        private Vector2 spawnRate = new Vector2(10, 30);

        [SerializeField] 
        private BackgroundObject.BackgroundObjectData[] backgroundObjects;


        public Vector2 SpawnRate => spawnRate;
        public BackgroundObject.BackgroundObjectData[] BackgroundObjects => backgroundObjects;
    }
}