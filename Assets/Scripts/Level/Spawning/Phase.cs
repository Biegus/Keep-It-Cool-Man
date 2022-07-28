using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
    
    
    
    
    
   /// <summary>
   /// Will be reused, but never two at the same time. Init and dispose should be markers from start to end of one lifecycle.
   /// Using might require a little boilerplate. Look for <see cref="LegacySpawnController"/> or <see cref="IndependentSpawnController"/> for reference.
   /// </summary>
    public interface ISpawnController
   {
       /// <summary>
       /// only to display, doesn't do anything with mechanics
       /// </summary>
       float GetTimeInfo();
        void Init(Phase phase);
        /// <summary>
        ///  //returns false when there's end of phase
        /// </summary>
       
        bool Update(Phase phase);
        void Dispose(Phase phase);
        #if UNITY_EDITOR
        /// <summary>
        /// Description will be used in editor only
        /// </summary>
        public string GetDescription(Phase phase);
        #endif
    }
   
   //This is drawn using custom gui. Adding certain attributes or  fields might not work expected
    [Serializable][AlwaysValidateRecursively]
    public class Phase : ISerializationCallbackReceiver
    {
        [Obsolete] [FormerlySerializedAs("time")] [SerializeField][Range(0,100)] private float legacyTime;
        [SerializeField] private bool useAllBeforeElements=false;
        [ValidateNotNull][ValidateRecursively] [FormerlySerializedAs("randomPhase")] [SerializeField] private PhaseElement[] elements = new PhaseElement[0];
        [ValidateNotNull][ValidateRecursively] [SerializeReference] [SerializeReferenceButton] private ISpawnController spawnController = null;
        
        private PhaseElement[] runtimeElements;

        [FormerlySerializedAs("spawnCooldown")] [SerializeField][HideInInspector] private float spawnCooldown = 0;

        public float Time => (spawnController?.GetTimeInfo() ?? legacyTime);// dirty :(
        public float LegacySpawnCoooldown => spawnCooldown;
        public IReadOnlyList<PhaseElement> Elements => elements;
        public ISpawnController SpawnController => spawnController;
        
#if UNITY_EDITOR
        [SerializeField]
        private string phaseName;
        public string PhaseName => phaseName;
 #endif

        //can contain repetition,  it is recommended to cast to set
        public IEnumerable<char> GetSymbolsDump()
        {
            return elements.SelectMany(item => item.GetSymbolSet().GetSymbolsDumpByFlag(SimpleDirectionFlag.Everything));
        }
        public void Init(Phase before)
        {
            if (useAllBeforeElements)
            {
                if (before == null)
                {
                    Debug.LogError("There was no before element to steal from");
                    runtimeElements = elements;
                }
                else
                    runtimeElements = elements.Concat(before.elements).ToArray();
               
            }
            else
            {
                runtimeElements = elements;
            }
            spawnController.Init(this);
          
        }
     
        public bool Update()
        {

            bool res= spawnController.Update(this);
            if(!res)
                spawnController.Dispose(this);
            return res;
        }
        public PhaseElement GetRandomElementUsingPriority()
        {
            if (this.runtimeElements.Length == 0)
                return null;
            return runtimeElements.GetRandomWeightItemFromValue(item => item.Priority,Randomer.Base.NextFloat(0,1));;
        }
        public DoneSpawnData Spawn(PhaseElement winner)
        {
            if (winner == null) return new DoneSpawnData(null, 0);
            SpawnField fieldWinner = Randomer.Base.NextRandomElement(winner.SpawnFieldCollections.Fields);
            int lineIndex = LevelManager.Current.DirectionsOrder.Array.IndexOf((StraightDirection)fieldWinner.Direction);
            if (lineIndex == -1)
            {
                Debug.LogWarning("Not defined direction");
                return default;
            }
            Vector2 start = LevelManager.Current.Lines[lineIndex].position;
            Vector2 end = LevelManager.Current.Lines[(lineIndex + 1) % 4].position;
            Vector2 pointInLine = start + (end-start)* Randomer.Base.NextFloat(fieldWinner.From,fieldWinner.To);

            float angle = winner.OverrideAngle == -1 ? GameManager.CurrentLevel.AngleError : winner.OverrideAngle;
            if (fieldWinner.TargetIndex >= LevelManager.Current.Targets.Count)
            {
                Debug.LogError(($"The target with id \"{fieldWinner.TargetIndex}\" is not registered, but there was attempt to access it, probably phase uses spawn field that requires more targets\n" +
                                $"Spawn field collection that was used: {winner.SpawnFieldCollections}"));
                return default;
            };
            var target = LevelManager.Current.Targets[fieldWinner.TargetIndex];
           return  winner.Spawn(pointInLine, (SimpleDirection)fieldWinner.Direction, LevelManager.Current.Parent, angle,LevelManager.Current.Targets[fieldWinner.TargetIndex]);
           
        }
     
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (spawnController == null && spawnCooldown!=0)
            {
                 spawnController= new LegacySpawnController();
                 (spawnController as LegacySpawnController).SpawnCooldown = spawnCooldown;
                 (spawnController as LegacySpawnController).Time = legacyTime;

            }
        }
    }
}