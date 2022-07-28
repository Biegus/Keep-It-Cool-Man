using System.Runtime.InteropServices;

namespace ConsoleKicm
{
    public static class Program// why not top level? namespace issues
    {
        public static void Main(string[] args)
        {
           
            GameSystem gameSystem = null;
            void RunGame()
            {
                gameSystem = new GameSystem(new(60,25), new GameLogic());
                gameSystem.GameLoop();
            }

            try
            {
                RunGame();
            }
            finally
            {
                gameSystem?.FinishWithOutput();
            }

        }

      
    }
}
