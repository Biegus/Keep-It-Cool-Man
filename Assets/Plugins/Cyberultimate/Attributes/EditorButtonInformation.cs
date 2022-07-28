#nullable enable
using System;
using UnityEngine;

namespace Cyberultimate.Unity
{
    public enum InspectorButtonSize
    {
        Normal,
        Medium,
        Thick
    }

    [Flags]
    public enum InspectorButtonLimits
    {
        NoLimits=0,
        NotInPlayMode=1<<0,
        NotInEditorMode=1<<1,
        NotIfObjectIsNotEnabled=1<<2,
        NotIfObjectIsEnabled=1<<3,
    }
    public class InspectorButtonInformationAttribute: PropertyAttribute
    {
        public string Name { get; }
        public string Method { get; }
        public InspectorButtonSize Size { get; }
        public  InspectorButtonLimits Limit { get; set; }
        public string? ArgumentDescription { get; set; } = null;
        public InspectorButtonInformationAttribute(string name,  string method, InspectorButtonSize size)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Size = size;
        }
    }
}