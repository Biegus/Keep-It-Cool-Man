using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyberultimate.Unity;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;
namespace LetterBattle.Utility.SharedTemplate
{
    [CreateAssetMenu(menuName = "TemplateAsset")]
    public class TemplatesAsset : ScriptableObject
    {
        
    #if UNITY_EDITOR// it will just be empty in build, it makes some matters easier


        public enum Mode
        {
            SystemObj,
            UnityObj,
        }
        [Serializable]
        public struct Id
        {
            public int Index;
            public Mode Mode;
            public static Id BuildUnity(int index)
            {
                return new Id()
                {
                    Index = index, Mode = Mode.UnityObj
                };
            }
            public static Id BuildSystemObj(int index)
            {
                return new Id()
                {
                    Index = index, Mode = Mode.SystemObj
                };
            }
        }
        
        [Serializable]
        public struct PackedArray<T>
        {
            public List<T> List;
            public static PackedArray<T> Build()
            {
                return new PackedArray<T>()
                {
                    List = new List<T>()
                };
            }
        }
        [Serializable]
        public struct List2D<T>
        {
            public List<PackedArray<T>> List;
        }
        private static readonly IReadOnlyList<NamedTemplateSystemObj> Empty = new List<NamedTemplateSystemObj>();
        [SerializeField] private bool debug = false;
        
        [SerializeField] private List2D<NamedTemplateSystemObj> systemTemplates = new List2D<NamedTemplateSystemObj>();
        [SerializeField] private List2D<NamedTemplateUnityObj> unityTemplates = new List2D<NamedTemplateUnityObj>();
      
        [SerializeField] private SerializedDictionary<string, Id> ids;

    
        [NaughtyAttributes.HorizontalLine(2f,EColor.Green,order = 99)]
        [NaughtyAttributes.ShowIf("debug")]
        [SerializeField] private string eGroupName;
        [NaughtyAttributes.ShowIf("debug")]
        [SerializeField] private int eValue;
        [NaughtyAttributes.ShowIf("debug")]
        [SerializeField] private string eValueName;
   
        [Button()]
        [NaughtyAttributes.ShowIf("debug")]
        private void AddViaEditor()
        {
            Add(eGroupName,eValue,eValueName);
        }
        private IPacker Pack(object obj)
        {
            return PackerFactory.Create(obj);
        }
        public void Add(string groupName, object value, string valueName)
        {
            Id id;
            if (!ids.TryGetValue(groupName, out id))
            {
                
                // could be refactor better
                if (value is UnityEngine.Object)
                {
                    var build = PackedArray<NamedTemplateUnityObj>.Build();
                    var list  = build.List;
                    unityTemplates.List.Add(build);
                    list.Add(new NamedTemplateUnityObj(value as UnityEngine.Object, valueName));
                    ids.Add(groupName,Id.BuildUnity( unityTemplates.List.Count-1));
                }
                else
                {
                    var build = PackedArray<NamedTemplateSystemObj>.Build();
                    var list  = build.List;
                    systemTemplates.List.Add(build);
                    list.Add(new NamedTemplateSystemObj( Pack( value),valueName));
                    ids.Add(groupName,Id.BuildSystemObj( systemTemplates.List.Count-1));
                }
                
            }
            else
            {
                if (id.Mode == Mode.SystemObj && value is UnityEngine.Object
                || id.Mode==Mode.UnityObj && !( value is  UnityEngine.Object))
                {
                    Debug.LogError($"{groupName} is polluted");
                    return;
                }
                if (id.Mode == Mode.UnityObj)
                {
                    unityTemplates.List[id.Index].List.Add(new NamedTemplateUnityObj(value as UnityEngine.Object, valueName));
                }
                else
                    systemTemplates.List[id.Index].List.Add(new NamedTemplateSystemObj(Pack(value),valueName));
            }
            
            
            EditorUtility.SetDirty(this);
        }
        public IReadOnlyList<INamedTemplate> Get(string groupName)
        {
            
            if (ids.TryGetValue(groupName, out Id id))
            {
                if (id.Mode == Mode.SystemObj)
                    return systemTemplates.List[id.Index].List;
                else
                    return unityTemplates.List[id.Index].List;
            }
            return Empty;
        }
        #endif
    }
}