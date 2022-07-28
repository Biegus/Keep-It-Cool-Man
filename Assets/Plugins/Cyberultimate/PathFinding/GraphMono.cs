#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Cyberultimate.Unity;
using Cyberultimate;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace Cyberultimate.Unity
{
    
    public sealed class GraphMono : MonoBehaviour,ISerializationCallbackReceiver
    {
        private class PositionNode : IComparable<PositionNode>
        {
            public Vector2Int Pos { get; }
            public float Weight { get; }
            public PositionNode(Vector2Int pos,float weight )
            {
                Pos = pos;
                Weight = weight;
            }

            public int CompareTo(PositionNode other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return Weight.CompareTo(other.Weight);
            }
        }

        public class GraphChangedArgs
        {
            public readonly struct SingleChange
            {
                public readonly bool Value;
                public readonly Vector2Int Position;

                public SingleChange(bool value, Vector2Int position)
                {
                    Value = value;
                    Position = position;
                }
            }
            /// <summary>
            /// Information about single change. If event is called after refreshing graph, this is null.
            /// </summary>
            /// <returns></returns>
            public SingleChange? Change { get; }

            public bool WasMultiplyChange => Change == null;
            public GraphChangedArgs(SingleChange? change)
            {
                Change = change;
            }
            
        }

        private static readonly float SQRT_OF_2 = Mathf.Sqrt(2);
        
        [SerializeField] private Vector2Int size=new Vector2Int(20,20);
        [SerializeField] private float nodeSize=1;
        [SerializeField] private ushort offset;
        [SerializeField] private List<LayerMask> obstacleMasks = new List<LayerMask>();
        [SerializeField] private bool setAsCurrentOnAwake = true;
        [SerializeField] private bool scanOnAwake = true;
        [SerializeField] private bool draw = true;
     
        
        #if UNITY_EDITOR
        [InspectorButtonInformation("Scan",nameof(RefreshGraph),InspectorButtonSize.Normal)]
        [SerializeField] private InspectorButton<NoneType> _e1button;

        private float? nodeSizeBf = null;
        private ushort? offsetBf = null;
        private Vector2Int? sizeBf = null;
    #endif
        /// <summary>
        /// This property doesn't affect anything. It's for easy reference if you only need to use one <see cref="GraphMono"/>
        /// </summary>
        public static GraphMono Main { get; set; } = null!;
        
        public float NodeSize
        {
            get => nodeSize;
            set
            {
                if (nodeSize == value) return;
                if (value <= 0)
                    ThrowBelowZeroException(nameof(value));
                nodeSize = value;
                OnNodeSizeChanged(this, value);
            }
        }

        public ushort Offset
        {
            get => offset;
            set
            {
                if (offset == value) return;
                offset = value;
                OnOffsetChanged(this, value);
            }
        }
        public Vector2Int Size
        {
            get => size;
            set
            {
                if (size == value) return;
                if (value.x <= 0 || value.y <= 0)
                    ThrowBelowZeroException(nameof(value));
                size = value;
                OnSizeChanged(this, value);
            }
        }

        private Vector2 NodeSizeV=> nodeSize*Vector2.one;

        public List<LayerMask> ObstacleMasks => obstacleMasks;
        
        public event EventHandler<SimpleArgs<float>> OnNodeSizeChanged = delegate { }; 
        public event EventHandler<SimpleArgs<Vector2Int>> OnSizeChanged = delegate { }; 
        public event EventHandler<SimpleArgs<Vector2>> OnPosChanged = delegate { }; 
        public event EventHandler<SimpleArgs<int>> OnOffsetChanged = delegate { };
        public event EventHandler<GraphChangedArgs> OnGraphChanged = delegate { };
        /// <summary>
        /// Doesn't include <see cref="ObstacleMasks"/>
        /// </summary>
        public event EventHandler<EventArgs> OnAnyGraphAffectingPropertyChanged = delegate { };
        
        private bool[,] graph;
        public bool GetGraph(int y, int x)
        {
            ThrowIfOutsideOfGrid(new Vector2Int( x, y));
            return graph[y, x];
        }

        public bool GetGraph(Vector2Int v)
        {
            ThrowIfOutsideOfGrid(v);
            return graph[v.y, v.x];
        }
        public void ChangeGraphData(Vector2Int pos,bool val)
        {
            ThrowIfOutsideOfGrid(pos);
            graph[pos.y, pos.x] = val;
            OnGraphChanged(this, new GraphChangedArgs(new GraphChangedArgs.SingleChange(val, pos)));
        }
        ///<summary></summary>
        /// <param name="graph">can be null</param>
        public static void SetCurrent(GraphMono graph)
        {
            Main = graph;
        }
        public Vector2Int ToPointOnGrid(Vector2 v)
        {
            return Vector2Int.RoundToInt(new Vector2( (v.x- this.transform.Get2DPos().x)/NodeSizeV.x,
                (v.y-this.transform.Get2DPos().y+NodeSizeV.y*(size.y-1))/NodeSizeV.y));
        }

        public Vector2 ToPointOutsideGrid(Vector2Int point)
        {
            return new Vector2(this.transform.Get2DPos().x+point.x*NodeSizeV.x,
                transform.Get2DPos().y-NodeSizeV.y*(size.y-1)+point.y*NodeSizeV.y);

        }
        public bool IsGridPointInGrid(Vector2Int point)
        {
            return point.x >= 0 && point.y >= 0 && point.x < graph.GetLength(1) && point.y < graph.GetLength(0);
        }

        public bool IsPointInGrid(Vector2 point)
        {
            return IsGridPointInGrid(ToPointOnGrid(point));
        }
        
        private  void Awake()
        {
            OnNodeSizeChanged += (s,  e) => OnAnyGraphAffectingPropertyChanged(this, EventArgs.Empty);
            OnSizeChanged += (s, e) => OnAnyGraphAffectingPropertyChanged(this, EventArgs.Empty);
            OnPosChanged += (s, e) => OnAnyGraphAffectingPropertyChanged(this, EventArgs.Empty);
            OnOffsetChanged += (s, e) => OnAnyGraphAffectingPropertyChanged(this, EventArgs.Empty);
        
;            
            if (setAsCurrentOnAwake)
                Main = this;
          
            if (scanOnAwake)
                RefreshGraph();
        }

        private void Update()
        {

            if (this.transform.hasChanged)
                OnPosChanged(this,this.transform.Get2DPos());
        }

        private void OnDrawGizmos()
        {
            if(!draw)
                return;
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2 point= new Vector2(x,y);
                    Vector2 cur = this.transform.Get2DPos() +
                                  new Vector2(point.x * NodeSizeV.x, -((size.y - point.y) * NodeSizeV.y) + NodeSizeV.y);
                    if (graph != null && graph.GetLength(0)==size.y && graph.GetLength(1)==size.x)
                    {
                        Gizmos.color = graph[y, x] ? (Color)new Clr(Color.white).CA(0.2f) : (Color)new Clr(Color.cyan){A= 0.5f};
                    }
                    else
                    {
                        Gizmos.color = new Clr(Color.white).CA(0.2f);
                    }
                    Gizmos.DrawCube(cur,
                        NodeSizeV);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(cur,NodeSizeV);
                 
                }
            }
            
        }

       
        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="diagonal">Determines if diagonal shoud be considered</param>
        /// <returns></returns>
        public IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, bool diagonal = true)
        {
           return  GetPathReversed(start, end, diagonal).Reverse();

        }
        /// <summary>
        /// It's faster that <see cref="GetPath"/> but results are in revesed order.
        /// </summary>
        /// <param name="diagonal">Determines if diagonal should be considered</param>
        /// <returns></returns>
        public IEnumerable<Vector2> GetPathReversed(Vector2 start,Vector2 end,bool diagonal=true)
        {
            MinPriorityQueue<PositionNode> queue= new MinPriorityQueue<PositionNode>();
            Dictionary<Vector2Int, float> costs= new Dictionary<Vector2Int, float>();
            Dictionary<Vector2Int, Vector2Int> parents= new Dictionary<Vector2Int, Vector2Int>();
            Vector2Int simplePoint = ToPointOnGrid(start);
            Vector2Int simpleEnd = ToPointOnGrid(end);
            if(!IsGridPointInGrid(simplePoint))
                throw new ArgumentException($"{nameof(start)} is out of grid");
            if(!IsGridPointInGrid(simpleEnd))
                throw new ArgumentException($"{nameof(end)} is out of grid");
            if (!GetGraph( simpleEnd.y,simpleEnd.x) || !GetGraph(simplePoint.y,simplePoint.x))
            {
                yield break;
            }
            queue.Enqueue(new PositionNode(simplePoint,0));
            costs[simplePoint] = 0;
            while (queue.Count != 0)
            {
                var front = queue.Dequeue();
                if (front.Pos == simpleEnd)
                    break;

                void CheckNeighbor(Vector2Int pos, float additionalCost)
                {
                    if (!GetGraph(pos.y, pos.x))
                        return;
                    float realCost = costs[front.Pos] + additionalCost;
                    if (costs.TryGetValue(pos, out float result))
                    {
                        if (result < realCost)
                            return;
                        
                    }

                    costs[pos] = realCost;
                    parents[pos] = front.Pos;
                    queue.Enqueue(new PositionNode(pos, realCost+Heuristic(pos,simpleEnd)));
                   
                  
                }
                //straights
                
                if(front.Pos.x>0)
                    CheckNeighbor(new Vector2Int(front.Pos.x-1,front.Pos.y),1);
                if(front.Pos.x<Size.x-1)
                    CheckNeighbor(new Vector2Int(front.Pos.x+1,front.Pos.y),1);
                if(front.Pos.y>0)
                    CheckNeighbor(new Vector2Int(front.Pos.x,front.Pos.y-1),1);
                if(front.Pos.y<Size.y-1)
                    CheckNeighbor(new Vector2Int(front.Pos.x,front.Pos.y+1),1);
                
                //diagonal

                if (diagonal)
                {
                    bool downY = front.Pos.y > 0 && GetGraph(front.Pos.y-1, front.Pos.x);
                    bool upY = front.Pos.y < Size.y - 1 &&
                               GetGraph(front.Pos.y + 1, front.Pos.x);
                    if (front.Pos.x > 0 &&  GetGraph(front.Pos.y,front.Pos.x - 1))
                    {
                  
                        if (downY)
                            CheckNeighbor(new Vector2Int(front.Pos.x - 1, front.Pos.y - 1), SQRT_OF_2);
                        if(upY)
                            CheckNeighbor(new Vector2Int(front.Pos.x - 1, front.Pos.y + 1), SQRT_OF_2);
                    }
                    if (front.Pos.x < Size.x - 1 &&  GetGraph(front.Pos.y,front.Pos.x +1))
                    {
                        if (downY)
                            CheckNeighbor(new Vector2Int(front.Pos.x + 1, front.Pos.y - 1), SQRT_OF_2);
                        if(upY)
                            CheckNeighbor(new Vector2Int(front.Pos.x + 1, front.Pos.y + 1), SQRT_OF_2);
                    }
                }

            }
            Vector2Int? node = simpleEnd;
            while (node != null)
            {
              
                if (parents.TryGetValue(node.Value, out var val))
                {
                    yield return this.ToPointOutsideGrid(node.Value);
                    node = val;
                }
                else
                {
                    node = null;
                }
            }
            

        }
        public void RefreshGraph()
        {
            //graph not inited or dirty
            if (graph == null|| graph.GetLength(0)!=size.y || graph.GetLength(1)!=size.x)
            {
                if(size.y<=0 || size.x<=0)
                    throw new InvalidOperationException($"{nameof(size)} was lower or equal than zero");
                graph= new bool[size.y,size.x];
            }
            for(int y=0;y<size.y;y++)
            for (int x = 0; x < size.x; x++)
            {
                Vector2 p= new Vector2(x,y);
                Vector2 cur = this.transform.Get2DPos() +
                              new Vector2(p.x * NodeSizeV.x,
                                  -((size.y - p.y) * NodeSizeV.y) + NodeSizeV.y);
                graph[y, x] =
                    (Physics2D.OverlapCircle(cur, offset * NodeSizeV.y / 2f + NodeSizeV.y / 2f) == null) &&
                    Physics2D
                        .OverlapBox(cur, NodeSizeV - Vector2.one * 0.001f, 0) == null;




            }
            OnGraphChanged(this,new GraphChangedArgs(null));
#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif

        }
        
        private static float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        private void ThrowBelowZeroException(string what)
        {
            throw new ArgumentException($"{what}'s value cannot be lower or equal than zero");
        }

        private void ThrowIfOutsideOfGrid(Vector2Int v)
        {
            if(!IsGridPointInGrid(v))
                throw new ArgumentOutOfRangeException("y,x","Point is out of grid");
        }
      
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            size.x = Mathf.Clamp(size.x, 0, 500);
            size.y = Mathf.Clamp(size.y, 0, 500);
            nodeSize = Mathf.Clamp(nodeSize, 0.0000001f, float.MaxValue);
#if UNITY_EDITOR
            if (offsetBf != this.offset)
                OnOffsetChanged(this, this.offset);
            if (sizeBf != this.sizeBf)
                OnSizeChanged(this, this.size);
            if (nodeSizeBf != this.nodeSizeBf)
                OnNodeSizeChanged(this, this.nodeSize);
            offsetBf = this.offset;
            sizeBf = this.size;
            nodeSizeBf = this.nodeSize;
#endif
        }
    }
}