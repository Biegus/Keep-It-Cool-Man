using System;
using System.IO;
namespace LetterBattle
{
    public class StoreSystem
    {
        public static string PATH { get; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"KICM");

    }
}