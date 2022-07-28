#nullable enable
namespace Cyberultimate.Unity.Internal
{
    internal class CorReturn: IWaitable
    {
        public object Value { get; }

        public CorReturn(object value)
        {
            Value = value;
        }
    }
}