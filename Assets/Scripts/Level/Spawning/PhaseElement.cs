using System;
using Cyberultimate;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace LetterBattle
{

    [Serializable][AlwaysValidateRecursively]
    public class PhaseElement : IValidator
    {

        [ValidateNotNull] [ValidateRecursively] [SerializeField] private BehaviorTable table;
        [ValidateNotNull] [SerializeField]  private SpawnFieldCollectionAsset spawnFieldCollections = null;
         [SerializeField] private LettersPackage overrideLetters = null;
        [FormerlySerializedAs("probability")] [SerializeField][Range(0,3)] private float priority;
      
        [SerializeField]private float overrideAngle = -1;
       
        [SerializeField]
        private CyberOptional<float> customCooldown = CyberOptional<float>.Empty;
        [FormerlySerializedAs("scaledColdown")] [SerializeField] private bool scaledCooldown = false;

        public BehaviorTable Table => table;

        public float OverrideAngle => overrideAngle;
        public float Priority
        {
            get => priority;
            set => priority = value;
        }
        //result will be null that this value is not supplied, tho in such game wouldn't work properly anyway
        public ILettersPackage GetSymbolSet()
        {
            return overrideLetters!=null ? overrideLetters : table.Behaviour.GetDefLetters();
        }
        public CyberOptional<float> CustomCooldown => customCooldown;
     
        public DoneSpawnData Spawn(Vector2 pos, SimpleDirection side,Transform parent, float angleError, Transform target)
        {
            var data= table.Spawn(pos, side, parent,angleError, target,overrideLetters);
            if (!scaledCooldown)
                data.AbsoluteCount = 1;
            return data;
        }
        
        public SpawnFieldCollectionAsset SpawnFieldCollections
        {
            get => spawnFieldCollections;
            set => spawnFieldCollections = value;
        }


         ValidateResult IValidator.Validate(string place)
        {
            if (overrideLetters is null && table.Behaviour.GetDefLetters() is null)
            {
                return ValidateResult.One($"Either behaviour or phaselement should have letterspack set. ({place})");
            }
            return ValidateResult.Ok;
        }
    }
}