using Cyberultimate;
using Utility;
namespace LetterBattle
{
    public class InputLetterArgs : AskerEventArgs<char,ActionLetter>
    {
        public char Letter { get; }
        public InputLetterArgs(char letter)
        {
            this.Letter = letter;
        }
    }
}