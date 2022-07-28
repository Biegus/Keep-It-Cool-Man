using Cyberultimate;
namespace LetterBattle
{
    public class DummyLettersPackage : ILettersPackage
    {

        public string Letters { get; }
        public DummyLettersPackage(string letters)
        {
            this.Letters = letters;
        }
        public string GetLetters(SimpleDirection side)
        {
            return Letters;
        }
    }
}