#nullable enable
using System.Collections.Generic;

namespace Cyberultimate.Unity
{
    public class WaitablePush: IWaitable
    {
        public IEnumerator<IWaitable> Element { get; }

        public WaitablePush(IEnumerator<IWaitable> element)
        {
            Element = element;
        }
    }
}