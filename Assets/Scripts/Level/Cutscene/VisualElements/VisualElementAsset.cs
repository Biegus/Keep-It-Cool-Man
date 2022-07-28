using UnityEngine;

namespace LetterBattle
{
    [CreateAssetMenu(fileName = "VisualElementAsset", menuName = "VisualElementAsset", order = 0)]
    public class VisualElementAsset : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField][NaughtyAttributes.MinValue(0)] private int visualPoint;

        public Sprite Sprite => sprite;

        [ SerializeReference, SerializeReferenceButton]
        private IVisualElementMovement movement;
        
        public IVisualElementMovement Movement => movement;
        public int VisualPoint => visualPoint;
    }
}