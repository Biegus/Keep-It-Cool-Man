#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace Cyberultimate.Unity
{
   
    public class BetterImage : Image
    {
        [SerializeField]
        public new bool preserveAspect;

        protected override void Awake()
        {
            base.Awake();
            base.preserveAspect = preserveAspect;
        }


    }

}
