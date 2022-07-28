#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LetterBattle.Utility;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
namespace LetterBattle.Editor
{
    public class EventDebugger : EditorWindow
    {
        private static PropertyInfo[] members;
        private bool[] foldout = null;
        private Superviser[] supervisers;
        private LinkedList<string> output = new LinkedList<string>(); // would be better with loop array

        static EventDebugger()
        {
            members = typeof(LEvents).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField)
                .Where(item=>item.PropertyType.GetGenericTypeDefinition()==typeof(CallableEvent<>))
                .ToArray();

        }
        
        [MenuItem("Kicm/EventDebugger")]
        private static void ShowWindow()
        {
            var window = GetWindow<EventDebugger>();
            window.titleContent = new GUIContent("Event Debugger");
            window.Show();
        }
        private void OnEnable()
        {
            foldout = new bool[members.Length];
            supervisers = new Superviser[members.Length];
        }
        private void Update()
        {
            Repaint();
        }
       
        private void OnDisable()
        {
            foreach (var superviser in supervisers)
            {
                superviser?.Dispose();
            }
        }
        private void Supervise(BaseCallableEvent value, PropertyInfo element, int i)
        {

            Unsupervise(value,element,i);
            var superviser = new Superviser(value, element.Name, i);
            superviser.OnEvent += OnSupervisedEventIsCalled;
            supervisers[i] = superviser;
        }
        private void Unsupervise(BaseCallableEvent value, PropertyInfo element, int i)
        {
            supervisers[i]?.Dispose();
            supervisers[i] = null;
        }
        private void InvertSupervision(BaseCallableEvent value, PropertyInfo element, int i)
        {
            if (supervisers[i] == null)
                Supervise(value,element,i);
            else Unsupervise(value,element,i);
        }
        public bool IsSupervised(int index)
        {
            return supervisers[index] != null;
        }
        
      
        private void OnSupervisedEventIsCalled(object sender, BaseCallableEvent ev)
        {
            Superviser viser =sender as Superviser;
            
            output.AddLast($"<color=#{Color.Lerp(Color.white,new Color(0.5f,0.5f,0.5f,1),((float)viser.Index)/(members.Length-1)).GetColorHex(false)}>{viser.Name} [{TimeSpan.FromSeconds(Time.time):ss\\.ff}] </color>");
            if (output.Count >= 5)
            {
                output.RemoveFirst();
            }
        }
      
        public class Superviser: IDisposable
        {
            public event EventHandler<BaseCallableEvent> OnEvent = delegate { };
            public BaseCallableEvent Ev { get; }
            public string Name { get; }
            public int Index { get; }
            public Superviser(BaseCallableEvent ev, string name, int index)
            {
                Ev = ev;
                ev.RegisterNonType((Action)OnEventInternal);
                Name = name;
                Index = index;
            }
            private void OnEventInternal()
            {
                OnEvent(this, Ev);
            }
            public void Dispose()
            {
                Ev.UnregisterNonType((Action)OnEventInternal);
            }
        }
        private void OnGUI()
        {


            DrawEvents();
            DrawCalls();
        }
        private void DrawEvents()
        {

            for (var index = 0; index < members.Length; index++)
            {
                var element = members[index];
                var value = (BaseCallableEvent)element.GetValue(LEvents.Base);
                int callabackCount = value.GetCallsCount();
                if (IsSupervised(index))
                    callabackCount--;
                GUILayout.BeginHorizontal();


                var style = new GUIStyle("button");
                style.fontSize = 15;
                
                if (supervisers[index] != null)
                {
                    style.normal.textColor = style.onHover.textColor = style.hover.textColor = style.onActive.textColor = style.active.textColor = Color.green;
                }

                if (GUILayout.Button("!", style, GUILayout.Width(15)))
                {
                    InvertSupervision(value, element, index);
                }

                GUILayout.BeginVertical();

                if (foldout[index] = EditorGUILayout.Foldout(foldout[index], $"{element.Name} [{callabackCount}]"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();


                    var delegates = value.GetInvocationList();
                    bool any = false;
                    foreach (var del in delegates)
                    {
                        if (del.Target.GetType() == typeof(Superviser))
                            continue;
                        any = true;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("X", GUILayout.Width(15)))
                        {
                            value.UnregisterNonType(del);
                        }

                        GUILayout.Label($" <color=green>{del.Target.GetType().Name}</color>:: <color=Purple>{del.Method.ToString()}</color>", new GUIStyle("label")
                        {
                            richText = true
                        });

                        GUILayout.EndHorizontal();
                    }
                    if (!any)
                    {
                        GUILayout.Label("No subscribers");
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

            }
        }
        private void DrawCalls()
        {

            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Calls:", EditorStyles.boldLabel);
            if (GUILayout.Button("Clear", GUILayout.Width(new GUIStyle("button").CalcSize(new GUIContent("Clear")).x)))
            {
                output.Clear();
            }
            GUILayout.EndHorizontal();
            GUILayout.TextArea((output as IEnumerable<string>).Reverse().Aggregate(new StringBuilder(), (b, v) => b.AppendLine(v)).ToString(), new GUIStyle("textArea")
            {
                richText = true
            });
        }
    }
}
#endif