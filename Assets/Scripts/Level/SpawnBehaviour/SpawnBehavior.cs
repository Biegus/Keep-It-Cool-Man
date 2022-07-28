using System;
using System.Collections;
using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
using System.Linq;
namespace LetterBattle
{
    public struct DoneSpawnData
    {
        /// <summary> first spawned object</summary>

        public GameObject Obj;
        /// <summary>
        ///  how much element will really be spawned, is used when spawning of certain item is set to be scaled with absolute count
        /// </summary>
        public int AbsoluteCount;
        public DoneSpawnData(GameObject obj, int absoluteCount)
        {
            this.Obj = obj;
            this.AbsoluteCount = absoluteCount;
        }
        public static DoneSpawnData One(GameObject result)
        {
            return new DoneSpawnData(result, 1);
        }
    }
    [Serializable]
    public class SpawnBehavior
    {
        
        [SerializeField]
        protected ComponentsCache cache;
        
        public ISideSpawnBehaviour[] SidesRef=null;
        protected virtual DoneSpawnData InternalSpawn(in SpawnData data)
        {
            var obj = UnityEngine.Object.Instantiate(data.Prefab,data.Parent,false);
            cache = new ComponentsCache(obj);
            obj.transform.SetAsFirstSibling();
            obj.transform.position = data.Pos;
            return DoneSpawnData.One(obj);
        }
        public DoneSpawnData SpawnBase(in SpawnData data)
        {
            return InternalSpawn(data);
        }
        public DoneSpawnData Spawn(in SpawnData data,ISideSpawnBehaviour except=null)
        {
            DoneSpawnData result= this.InternalSpawn(data);
            if(SidesRef!=null)
                foreach (var side in SidesRef)
                {
                    if (side != except)
                        result.AbsoluteCount += side.PushEffect(data, cache, this);
                }
            return result;
        }
        public virtual ILettersPackage GetDefLetters()
        {
            return null;
        }
    }
}