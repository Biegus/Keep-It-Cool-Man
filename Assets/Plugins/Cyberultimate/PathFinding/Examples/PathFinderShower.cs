#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyberultimate;
using Cyberultimate.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
namespace Cyberultimate.Unity.Examples
{
    public class PathFinderShower : MonoBehaviour
    {
        [SerializeField] private Transform point;
        [SerializeField] private bool diagonal = true;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
        
            if (point == null) return;
            var path = GraphMono.Main.GetPath(this.transform.position, point.position, diagonal).ToList();
           
            Vector2 last = this.transform.position;
            for(int x=0;x<path.Count;x++)
            {
                Vector2 actual = path[x];
                Handles.DrawBezier(last,actual,last,actual, Color.red,null,10);
                last = actual;
            }
        }

     
    }
}
#endif