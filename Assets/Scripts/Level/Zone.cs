using UnityEngine;
namespace LetterBattle
{
    public class Zone : MonoBehaviour
    {
        [SerializeField] private Planet planet;
        public Planet Planet => planet;
    }
}