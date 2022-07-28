using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Cyberultimate;

using UnityEngine;
namespace LetterBattle
{

    [DataContract]
    public class PlayerState
    {
        /// <summary>
        /// Represents the higher level that can be play, at the default it's zero ( you can play level 0 but u didn't finished it)
        /// </summary>
        [DataMember]
        public int LevelUnlocked { get; set; } = 0;
      
        public List<ScoreData> Scores => _Scores;
        
        [DataMember]
        private  List<ScoreData> _Scores = new List<ScoreData>();
        
        [DataMember] 
        public bool HasSeenStartCutscene { get;  set; }
      
      
        /// <summary>
        ///   This function can consider level not played even if it is lower than LevelUnlocked ( usually when LevelUnlocked what achieve in "hacky" way
        /// </summary>
        public bool WasLevelFinished(int level)
        {
            if (level < 0) throw new ArgumentException("Level has to be greater than 0 to dermine whether was finished");
            if (_Scores.Count <= level)
                return false;
            else return _Scores[level]!= ScoreData.None;
            

        }
        public bool IsLevelUnlocked(int chapter, int level)
        {
           return  GameManager.PlayerState.LevelUnlocked >= GameAsset.Current.FromCertainToRaw(chapter, level);
        }

        public bool IsLevelUnlocked(int level)
        {
           
            return GameManager.PlayerState.LevelUnlocked >= level;
        }

        public override string ToString()
        {
            return $"Level={LevelUnlocked}, Score=[{Scores.Aggregate(new StringBuilder(), (b, v) => b.Append($"{v}, "))}]";
        }
      


    }
}