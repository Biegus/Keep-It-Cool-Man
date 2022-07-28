using System;
using System.Collections.Generic;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    [CreateAssetMenu(fileName = "Chapter", menuName = "Chapter", order = 0)]
    public class ChapterAsset : ScriptableObject
    {
      
        [ValidateNotNull]
        [SerializeField]private List<LevelAsset> levels = new List<LevelAsset>();
        public IReadOnlyList<LevelAsset> Levels => levels;
        #if UNITY_EDITOR
        [SerializeField] private string defaultLevelPath;
        public string DefaultLevelPath => defaultLevelPath;
        public List<LevelAsset> GetEditorRef()
        {
            return levels;
        }
        #endif
        
    }
}