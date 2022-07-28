using System;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle.Utility
{
    //LIST OF THINGS THAT COULD BE DONE TO IMPROVE WHOLE function
    //this function is for now not finished, and can cause minor problems
    //TODO: handling of type collisions
    //TODO: easy way to browse and also remove values and groups
    //TODO: allow to get from multiple set of groups
    //TODO: read group only ( not very important or useful tho)
    
    [AttributeUsage(AttributeTargets.Field)]
    public class UseSharedTemplateAttribute : PropertyAttribute
    {
        #if UNITY_EDITOR // no need to waste resources when built
        public string GroupName { get; }
        #endif
        public UseSharedTemplateAttribute(string groupName)
        {
            #if UNITY_EDITOR
            this.GroupName = groupName;
             #endif
           
        }
    }
}