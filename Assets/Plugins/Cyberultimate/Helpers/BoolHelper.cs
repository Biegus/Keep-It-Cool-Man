#nullable enable
namespace Cyberultimate
{
    public class BoolHelper
    {
        public static bool BetterParse(string? p)
        {

            if (p == null)
                return false;
            p = p.Trim();
            return p switch
            {
                "0" => false,
                "1" => true,
                _ => bool.Parse(p)
            };
        }
    }
}