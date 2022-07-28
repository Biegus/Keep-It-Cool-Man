#nullable enable
using System;

namespace Cyberultimate
{
    [Obsolete]
    public class BoolResolver
    {
        public Func<bool> Func { get; }
        public BoolResolver(Func<bool>? predict=null)
        {
            Func = predict??(()=>true);
        }
        public void Block(object? sender,  BoolResolverArgs resolver)
        {
            resolver.SendSignal(Func());
        }

        public static bool Check(Action<BoolResolverArgs> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            BoolResolverArgs args= new BoolResolverArgs();
            action(args);
            return args.NoSignal;
        }
        
    }
}