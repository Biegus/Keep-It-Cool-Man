#nullable enable
using System;
using UnityEngine;

namespace Cyberultimate.Unity.Internal
{
    public class InternalCyberCoroutineController: MonoSingleton<InternalCyberCoroutineController>
    {
        public CorController Controller { get; } = new CorController();
        
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            GameObject g = new GameObject {name = "BaseCoroutineController"};
            g.AddComponent<InternalCyberCoroutineController>();
            UnityEngine.Object.DontDestroyOnLoad(g);
            g.hideFlags = HideFlags.HideInHierarchy;
        }

        private void Update()
        {
            Controller.UpdateState(CorLoopState.Update);
        }
        private void FixedUpdate()
        {
            Controller.UpdateState(CorLoopState.Fixed);
        }
        private void LateUpdate()
        {
            Controller.UpdateState(CorLoopState.Late);
        }
    }
}