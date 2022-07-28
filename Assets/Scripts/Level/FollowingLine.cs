using System;
using System.Collections.Generic;
using System.Numerics;
using Cyberultimate.Unity;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
namespace LetterBattle
{
    [RequireComponent(typeof(LineRenderer))]
    public class FollowingLine : MonoBehaviour
    {
        [SerializeField] private LinkedList<Transform> points= new LinkedList<Transform>();
        [SerializeField] private bool dieOnNone = false;
        private int seed = Randomer.Base.NextInt();
        public bool DieOnNone
        {
            get => dieOnNone;
            set => dieOnNone = value;
        }
        public LineRenderer LineRenderer { get; private set; }
        public void PushPoint(Transform point)
        {
            points.AddLast(point);
        }
        private void Awake()
        {
            this.LineRenderer = this.GetComponent<LineRenderer>();
        }
        public static FollowingLine Spawn()
        {
            var line = UnityEngine.Object.Instantiate(GameAsset.Current.LineRenderConnectionPrefab);
            line.useWorldSpace = true;
            line.startColor = line.endColor = Color.red;
            line.sortingOrder = -10;
            line.startWidth = line.endWidth = 0.03f;
            line.positionCount = 0;
            return line.gameObject.AddComponent<FollowingLine>();
        }
        private void Update()
        {

            
            var node = points.First;
            while(node!=null)
            {
                var next = node.Next;
                if (node.Value == null) // dead x_x
                    points.Remove(node);
                node = next;
            }
            
            if (points.Count == 0)
            {
                if (dieOnNone)
                    Destroy(this.gameObject);

                return;
            }
            LineRenderer.positionCount = points.Count*2-1;
            int index = 0;
            foreach (Transform point in points)
            {

                Vector2 pos = point.Get2DPos();
                LineRenderer.SetPosition(index, point.Get2DPos());
                index+=2;
            }
            //   *     *
            //   *     *
            //n--o--k--o--m when  time=0 + pik
            //   *     *
            //   *     *
            //
            //   *    *
            //   *    *
            // n *  k * m
            //  \*/  \*/     when time= pi/2 + pik (absolute)
            //   o    o
            // this wiggle effect is achieved by adding additional point. It is exactly in the middle of two other points but it's moved on axis perpendicular (marked as *) to vector between two points. 
            LinkedListNode<Transform> nd = points.First;
            Randomer rand = new Randomer(seed);
            for (int i = 1; i < LineRenderer.positionCount; i+=2)
            {
                Vector2 cur = nd.Next.Value.Get2DPos();
                Vector2 bf = nd.Value.Get2DPos();
                Vector2 progressVector = (cur - bf)/2;
                Vector2 perpendicular = progressVector.GetRotated(90).normalized;
                Vector2 movedProgressVector = progressVector + perpendicular * Mathf.Sin(Time.time*10+rand.NextFloat(0,Mathf.PI))/10;
                
               
                LineRenderer.SetPosition(i, movedProgressVector + bf);
                nd = nd.Next;
            }

        }

    }
}