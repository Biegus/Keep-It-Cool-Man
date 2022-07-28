using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
namespace LetterBattle
{
	//This is drawn using custom gui. Adding certain attributes or  fields might not work as expected
	[CreateAssetMenu(fileName = "level", menuName = "LevelAsset", order = 0)]
    public class LevelAsset : ScriptableObject, IValidator
    {
	    [FormerlySerializedAs("cutscene")] [SerializeField] private CutsceneAsset preCutscene = null;
	     [SerializeField] private CutsceneAsset afterCutscene = null;
	    [SerializeField] private BackgroundTypeAsset background = null;
	    [FormerlySerializedAs("useAsset")] [SerializeField]
	    private bool useAssetInRankRules=false;
	    [FormerlySerializedAs("rankRules")] [SerializeField] private RankRules rankRulesLiteral = null;
	    [FormerlySerializedAs("rankRules")] [SerializeField] private RankRulesAsset rankRulesAsset = null;
        [ValidateRecursively] [SerializeField] private SReadonlyArray<Phase> phases = new SReadonlyArray<Phase>();
        [SerializeField] private SerializedScene scene;
        [ValidateNotNull] [SerializeField][PrefabObjectOnly] private GameObject[] additionalObjectsToSpawn = new GameObject[0];
        [Header("Setttings")]
        [SerializeField] private bool punishForWrong = true;
        [SerializeField] private bool disableRanks = true;
        [SerializeField][Range(0.1f,30f)] private float startHp = 5;
        [SerializeField] [Range(0, 90f)] private float angleError = 20;
        [SerializeField] private string startText = string.Empty;
        [SerializeField] private bool skip = false;
        [SerializeField] private AudioClip customMusic;
        [SerializeField]
        [TextArea]
        private string levelName = null;
        public bool PunishForWrong => punishForWrong;
        public int SceneId => scene.BuildIndex;

        public AudioClip CustomMusic => customMusic;
        public IReadOnlyCollection<GameObject> AdditionalObjectsToSpawn => additionalObjectsToSpawn;
        public float StartHp => startHp;
        public float AngleError => angleError;
        public CutsceneAsset PreCutscene => preCutscene;

        public CutsceneAsset AfterCutscene => afterCutscene;
        public BackgroundTypeAsset Background => background;
        public bool Skip => skip;
        public string StartText => startText;
        public IRatingRules RankRules => (disableRanks)? (IRatingRules) RatingRulesDummy.Instance :  ( (useAssetInRankRules)? rankRulesAsset :  (IRatingRules)rankRulesLiteral);
        
        [SerializeField] [HideInInspector] private int editorSelectedPhase;

        //the reference might be changed
        public ReadOnlyCollection<Phase> Phases => phases.Array;
        public float Time => Phases.Sum(item => item.Time);

        public string LevelName => levelName;

        public int? ValidateIndividualityOfController(Phase given)
        {
	        for (var i = 0; i < this.Phases.Count; i++)
	        {
		        var element = this.Phases[i];
		        if (element == given) continue;
		        if (element.SpawnController == given.SpawnController)
		        {
			        return i;
		        }
	        }
	        return null;
        }

        //this can be slow
        public HashSet<char> GetSymbols()
        {
	        return new HashSet<char>( phases.Array.SelectMany(item => item.GetSymbolsDump()));
        }
        public override string ToString()
		{
            return levelName;
		}
        public ValidateResult Validate(string place)
        {
	        #if UNITY_EDITOR
	        if(scene.SceneReference==null) return ValidateResult.One($"{place}Scene is null");
#endif
	        return ValidateResult.Ok;
	       
        }
    }
}