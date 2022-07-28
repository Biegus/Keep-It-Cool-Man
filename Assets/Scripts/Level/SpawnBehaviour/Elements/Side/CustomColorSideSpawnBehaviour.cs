using LetterBattle.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    public class CustomColorSideSpawnBehaviour : ISideSpawnBehaviour
    {
        [SerializeField] [UseSharedTemplate("CustomColorSideSpawnBehaviour/Color")] private Color color;

        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            cache.GameObject.GetComponentsInChildren<Graphic>().Foreach(item=>item.color=color);
            return 0;
        }
    }
}