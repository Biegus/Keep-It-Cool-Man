using System.Collections.Generic;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    public static class Effects
    {
       
        public static CyberCoroutine ShakeObj(GameObject obj, Vector2 basePos,int  shakingAmount = 10, float power = 0.05f, float delay = 0.01f, UnityEngine.Object limit=null)
        {
            return CorController.Base.Start(ShakingObj(obj, basePos, shakingAmount, power, delay), limit);
        }
        private static IEnumerator<IWaitable> ShakingObj(GameObject obj,Vector2 basePos, int shakingAmount = 10, float power = 0.05f, float delay = 0.01f)
        {
            float z = obj.transform.position.z;
            for (int x = 0; x < shakingAmount; x++)
            {
                Vector2 nwPos = basePos + power * Randomer.Base.NextDirection();

                if (obj == null)
                    yield break;

                obj.transform.position = new Vector3(nwPos.x, nwPos.y, z);

                yield return Yield.Wait(delay, ignoreTimeScale: true);
            }
            obj.transform.position = new Vector3(basePos.x, basePos.y, z);

        }
    }
}