#nullable enable
using Cyberultimate.Unity.Internal;
using UnityEngine;

namespace Cyberultimate.Unity
{
    public interface IWaitable{}
    public  interface  IWaitableCondictional: IWaitable
    {
        
        bool IsReady();
        void Pause();
        void Resume();
       
    }
   
    
    
}