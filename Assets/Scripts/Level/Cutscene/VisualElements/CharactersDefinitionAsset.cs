using UnityEngine;

namespace LetterBattle
{
    [CreateAssetMenu(fileName = "VisualElementsDefinitionAsset", menuName = "VisualElementsDefnition", order = 0)]
    public class CharactersDefinitionAsset : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, CharacterAsset> characters;

        public CharacterAsset Get(string key)
        {
            if (!characters.ContainsKey(key))
                return null;
            return characters[key];
        }
    }
}