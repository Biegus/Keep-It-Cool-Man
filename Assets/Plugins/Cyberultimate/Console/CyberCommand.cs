#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate.Unity
{
    public class CyberCommand:ICommand
    {
        public IMetaData MetaData { get;}
        public Func<string[],string?> Delegate { get; }
        public CyberCommand(IMetaData atr, Delegate method)
        {
            this.MetaData = atr ?? throw new ArgumentNullException(nameof(atr));
        
            if (method.Method.ReturnType == typeof(void))
            {
                Action<string[]> baseDlg =(Action<string[]>) method;
                Delegate = (args) => { baseDlg(args); return null; };
            }
            else
                Delegate= (Func<string[],string>)method;
        }
        
    
        public string? Call(string[] args)
        {
            return this.Delegate(args);
        }
        public override string ToString()
        {
            return  ($"{MetaData.Name}" +
                     $"{this.MetaData.ArgumentDescription.Aggregate(new StringBuilder(), (b, e) => b.Append($" [{e}] ")).ToString()}") ;
        }
    }
}

