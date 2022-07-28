#nullable enable
namespace Cyberultimate.Unity.Internal
{
    internal class EscapeThread : IWaitable
    {
        public bool UseThreadPool { get; }

        public EscapeThread(bool useThreadPool)
        {
            UseThreadPool = useThreadPool;
        }
    }
}