using DG.Tweening;
using UnityEngine;
namespace LetterBattle
{
 
    public static class InitTween
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            DOTween.SetTweensCapacity(1000,100);
        }
    }
}