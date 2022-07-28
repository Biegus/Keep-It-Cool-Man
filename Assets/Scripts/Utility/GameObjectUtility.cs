using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle.Utility
{
    public static  class GameObjectUtility
    {
        public static void SetAllGraphicsAlpha(this GameObject gameObject, float value)
        {
            foreach (var component in gameObject.GetComponentsInChildren<Graphic>())
            {
                component.color = new Clr(component.color, value);
            }
        }
    }
}