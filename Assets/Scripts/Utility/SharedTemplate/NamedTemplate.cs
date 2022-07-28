using System;
using UnityEngine;
namespace LetterBattle.Utility.SharedTemplate
{
    public interface INamedTemplate
    {
        public string Name { get; }
        public object Value { get; }
    }
  
    [Serializable]
    public class NamedTemplateUnityObj : INamedTemplate
    {
       [SerializeField]// it's fun but it works, when value is UnityEngine.Object
       
        
        public UnityEngine.Object Value;
        public string Name;
     
        public NamedTemplateUnityObj(UnityEngine.Object value, string name)
        {
            Value = value;
            Name = name;
        }
        string INamedTemplate.Name => Name;
        object INamedTemplate.Value => Value;
    }
    public interface IPacker
    {
        object GetObj();
        void SetObj(object obj);
    }
    // unit doesn't allow to use generic in pair with serialize reference
    public class IntPacker: Packer<int>{}
    public class FloatPacker: Packer<float>{}
    public class StringPacker: Packer<string>{}
    public class ColorPacker : Packer<Color>{};
    public class PackerFactory
    {
        public static IPacker Create(object obj)
        {
            IPacker packer;
            packer = obj switch
            {
                int v => new IntPacker(),
                float v => new FloatPacker(),
                string v => new StringPacker(),
                Color v=> new ColorPacker(),
            };
            packer.SetObj(obj);
            return packer;
        }
    }
    
    [Serializable]
    public class Packer<T> : IPacker
    {
        [SerializeField]
        public T Value;
        public object GetObj()
        {
            return Value;
        }
        public void SetObj(object obj)
        {
            Value = (T)obj;
        }
    }
    [Serializable]
    public class NamedTemplateSystemObj : INamedTemplate
    {
        [SerializeReference]
        
        private  IPacker packer;
        public string Name;
        public object Value => packer.GetObj();
     
        public NamedTemplateSystemObj(IPacker packer, string name)
        {
            this.packer = packer;
            Name = name;
        }
        string INamedTemplate.Name => Name;
        object INamedTemplate.Value => Value;
    }
   
    
}