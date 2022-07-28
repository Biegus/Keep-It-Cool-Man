#nullable enable
namespace Cyberultimate.Unity.Internal
{
    internal class StateChange : IWaitable
    {
        public CorLoopState Loop { get; }
        public StateChange(CorLoopState loop)
        {
            Loop = loop;
        }
    }
}