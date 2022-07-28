using System;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle
{
    public class LaserManager
    {
        public static LineRenderer Spawn(Vector2 start, Vector2 end, LineRenderer linePrefab = null, Transform keepTrack = null)
        {
            linePrefab ??= MonoLaserManager.Current.Prefab;
            var line = UnityEngine.Object.Instantiate(linePrefab);
            float baseWidth = line.widthMultiplier;
            var linePos = line.transform.Get2DPos();
            line.SetPositions(new Vector3[]
            {
                start-linePos, end-linePos
            });
            line.material.SetFloat("_StartTime", Time.time);
            float startW = line.startWidth;
            DOVirtual.Float(0f, 1f, 1, (value) =>
                {
                    //  line.material.SetFloat("_WaveScale",value*0.7f + 0.3f);
                    line.startWidth = line.endWidth = (1 - value * value * value) * startW;
                    if(keepTrack!=null)
                        line.SetPosition(0,keepTrack.Get2DPos()- linePos);
                   
                }).SetLink(line.gameObject)
                .OnComplete(() =>  UnityEngine.Object.Destroy(line.gameObject));
            return line;
        }
    }
    public   class MonoLaserManager : MonoSingleton<MonoLaserManager>
    {
        [SerializeField] private LineRenderer prefab;
        public LineRenderer Prefab => prefab;
      

    }
}