using System.Collections.Generic;
using System.Text;
using Cyberultimate;
using LetterBattle.Utility;
namespace LetterBattle
{
    public interface ILettersPackage
    {
        string GetLetters(SimpleDirection side);
    }
    public static class ILettersPackageExtension
    {
        public static IEnumerable<char> GetSymbolsDumpByFlag(this ILettersPackage lettersPackage, SimpleDirectionFlag sides)
        {
          
            for (int i = 0; i <= 3; i++)
            {
                var flag = (SimpleDirectionFlag)(1 << i);
                if ((sides & flag) == flag)
                {
                    foreach (var el in lettersPackage.GetLetters((SimpleDirection)flag))
                        yield return el;
                }
            }
        }
        public static HashSet<char> GetLettersByFlagAsSet(this ILettersPackage lettersPackage, SimpleDirectionFlag sides)
        {
            return new HashSet<char>(GetSymbolsDumpByFlag(lettersPackage, sides));
        }
    }
}