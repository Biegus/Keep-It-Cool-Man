#nullable enable
namespace Cyberultimate.Unity
{
    public interface IMetaData
    {
        public string Name { get; }
        public string[] ArgumentDescription { get; }
        public GameState GameState { get; }
    }
}