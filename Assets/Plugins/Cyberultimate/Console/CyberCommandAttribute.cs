#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate.Unity
{
    public  sealed class CyberCommandAttribute:Attribute,IMetaData
    {
        public string Name { get; }
        public string[] ArgumentDescription { get; }
        public GameState GameState { get;  }
        

        public CyberCommandAttribute(string name,GameState state,params string[]? descr )
        {
          
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ArgumentDescription = descr ?? new string[0];
            GameState = state;
        }
        public CyberCommandAttribute(string name,params string[]? descr )
        :this(name,GameState.Both,descr){}
       
 
    
    }
}

