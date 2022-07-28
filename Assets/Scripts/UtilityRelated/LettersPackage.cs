using Cyberultimate;
using UnityEngine;

namespace LetterBattle
{
    
    [CreateAssetMenu(fileName = "LettersPackage", menuName = "LettersPackage", order = 0)]
    public class LettersPackage : ScriptableObject, ILettersPackage
    {
        [SerializeField] private bool useSameForEveryone=false; 
        [NaughtyAttributes.HideIf (nameof(useSameForEveryone))]
        [SerializeField][NaughtyAttributes.Label("Value")] private DirectionPackage<string> package;
        [SerializeField][NaughtyAttributes.Label("Value")][NaughtyAttributes.ShowIf(nameof(useSameForEveryone))] private string single = "";
     
        public string GetLetters(SimpleDirection side)
        {
            if (useSameForEveryone) return single;
            else return package[side];
        }
    }
}