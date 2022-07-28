#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace Cyberultimate.Unity
{
    [Serializable]
    public class NoneType{}
   
    [Serializable]
    public class InspectorButton<T>
    {
        // ReSharper disable once NotAccessedField.Local
        
        [SerializeField] private T input = default!;
    }
}