using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
 
    public interface ICutsceneData
    {
        public AudioClip Clip { get; }
        public IReadOnlyList<CutsceneElement> Elements { get; }
        public int Back { get; }
     

    }
    [CreateAssetMenu(fileName = "CutsceneAsset", menuName = "CutsceneAsset", order = 0)]
    public partial class CutsceneAsset : ScriptableObject,ICutsceneData
    {
        
        [Header("General")]
        [SerializeField] private AudioClip clip;

        [FormerlySerializedAs("asset")] [SerializeField] private CutsceneElement[] elements = new CutsceneElement[0];
        [SerializeField] private int back=-1;
        public IReadOnlyList<CutsceneElement> Elements => elements;
        public AudioClip Clip => clip;
        public int Back => back;
    }
    
}