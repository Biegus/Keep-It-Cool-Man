using UnityEngine;
namespace LetterBattle
{
    public static class ParticleHelper
    {
        public static GameObject Spawn(GameObject obj, Vector2 pos, Transform parent = null)
        {
            if (obj == null) return null;
            GameObject temp = null;
            if (parent != null)
            {
                temp = UnityEngine.Object.Instantiate(obj, parent);
            }
            else
            {
                temp = UnityEngine.Object.Instantiate(obj);
            }
            temp.transform.position = pos;
            return temp;
        }
    }
}