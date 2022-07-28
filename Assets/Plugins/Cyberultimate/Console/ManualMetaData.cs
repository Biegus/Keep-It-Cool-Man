#nullable enable
using System;
namespace Cyberultimate.Unity
{
   
    public class ManualMetaData : IMetaData
    {
        
        public string Name { get; }
        public string[] ArgumentDescription { get; }
        public GameState GameState { get; }

        public ManualMetaData(string name, string[] argumentDescription, GameState gameState)
        {
            Name = name;
            ArgumentDescription = argumentDescription ?? new string[0];
            GameState = gameState;
        }
    
    }
}