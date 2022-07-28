#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyberultimate.Unity
{
  
    public static class TransformExtension
    {

        public static void LookAt2D(this Transform transform, Vector2 target)
        {
            transform.rotation = GetLookAt2DAsQuaternion(transform.position, target);
        }
        public static float GetLookAt2d(Vector2 pos,Vector2 target)
        {
            Vector2 dir = (Vector2)target - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle;
        }

        public static Quaternion GetLookAt2DAsQuaternion(Vector2 pos, Vector2 target)
        {
            return Quaternion.AngleAxis(GetLookAt2d(pos, target), Vector2.zero);
        }
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            return transform.OfType<Transform>();
        }
        public static Vector2 Get2DPos(this Transform transform)
        {
            return (Vector2)transform.position;
        }
        public static Vector2 Get2DScale(this Transform transform)
        {
            return (Vector2)transform.localScale;
        }
        public static void KillAllChildren(this Transform transform)
        {
            foreach (Transform item in transform.GetChildren())
            {
               
                UnityEngine.Object.Destroy(item.gameObject);
               
            }
        }

        public static void KillAllChildrenExcept(this Transform transform, Func<Transform, bool> func)
        {
            foreach (Transform item in transform)
                if (func(item) == false)
                    UnityEngine.Object.Destroy(item.gameObject);
        }

    }

}
