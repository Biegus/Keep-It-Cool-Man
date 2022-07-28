using Cyberultimate;
using UnityEngine;
namespace LetterBattle
{
    public struct SpawnData
    {
        public Vector2 Pos;
        public SimpleDirection Side;
        public Transform Parent;
        public GameObject Prefab;
        public Vector2 Direction;
        public Transform Target;
        public ILettersPackage CustomLetters;
        public SpawnData(Vector2 pos, SimpleDirection side, 
            Transform parent, GameObject prefab, Vector2 direction,
            Transform target, ILettersPackage customLetters)
        {
            Pos = pos;
            this.Side = side;
            Parent = parent;
            Prefab = prefab;
            Direction = direction;
            this.Target = target;
            this.CustomLetters = customLetters;
        }
        public SpawnData(in SpawnData other)
        :this(other.Pos,other.Side,other.Parent,other.Prefab,other.Direction,other.Target,other.CustomLetters){}
        
    }
}