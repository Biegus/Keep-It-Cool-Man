#if UNITY_EDITOR
using Egl= UnityEditor.EditorGUILayout;
using Gl= UnityEngine.GUILayout;
using Eg= UnityEditor.EditorGUI;
using System;
using System.Linq;
using System.Reflection;
using Cyberultimate.Editor;
using LetterBattle.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;
namespace LetterBattle.Editor
{
    public class LevelAssetSoftDrawer
    {
         private readonly struct Properties
        {
            public readonly SerializedProperty 
                Phases,
                Scene,
                AdditionalObjectsToSpawn,
                PunishForWrong,
                StartHp,
                LevelName,
                EditorSelectedPhase,
                Cutscene,
                Background,
                AngleError,
                DisableRanks,
                RankRules,
                StartText,
                UseRankRulesAsset,
                Skip,
                AfterCutscene,
                CustomMusic,
                RankRulesAsset;
            public Properties(SerializedObject obj)
            {
                Phases = obj.FindProperty("phases").FindPropertyRelative("array");
                Scene = obj.FindProperty("scene");
                AdditionalObjectsToSpawn = obj.FindProperty("additionalObjectsToSpawn");
                PunishForWrong = obj.FindProperty("punishForWrong");
                StartHp = obj.FindProperty("startHp");
                AngleError = obj.FindProperty("angleError");
                LevelName = obj.FindProperty("levelName");
                EditorSelectedPhase = obj.FindProperty("editorSelectedPhase");
                Cutscene = obj.FindProperty("preCutscene");
                AfterCutscene = obj.FindProperty("afterCutscene");
                Background = obj.FindProperty("background");
                RankRules = obj.FindProperty("rankRulesLiteral");
                RankRulesAsset = obj.FindProperty("rankRulesAsset");
                UseRankRulesAsset = obj.FindProperty("useAssetInRankRules");
                StartText = obj.FindProperty("startText");
                DisableRanks = obj.FindProperty("disableRanks");
                Skip = obj.FindProperty("skip");
                CustomMusic = obj.FindProperty("customMusic");
            }

        }
        private readonly struct PhaseProperties
        {
            public readonly SerializedProperty LegacyTime,
                SpawnController,
                Elements,
                UseAllFromBefore,
                PhaseName;
            public PhaseProperties(SerializedProperty obj)
            {
                LegacyTime = obj.FindPropertyRelative("time");
                SpawnController = obj.FindPropertyRelative("spawnController");
                Elements = obj.FindPropertyRelative("elements");
                UseAllFromBefore = obj.FindPropertyRelative("useAllBeforeElements");
                PhaseName = obj.FindPropertyRelative("phaseName");
            }
        }
        public SerializedObject SerializedObject { get; private set; }
        public int SelectedPhase { get; private set; }= 0;
        public LevelAsset Target { get; private set; }
        private Properties properties;

        private bool requireValidationToStart = true;
        private GUILayoutOption height;
        private ReorderableList phaseList;
        private GUIStyle boldThickLabel = null;
        private LevelAsset instance;
        public void Change(SerializedObject obj, LevelAsset target)
        {
            ChangeInternal(obj,target);
        }
        public LevelAssetSoftDrawer(SerializedObject obj, LevelAsset target)
        {
           
            ChangeInternal(obj,target);
            instance = (this.SerializedObject.targetObject as LevelAsset);
        }
        private  void ChangeInternal(SerializedObject obj, LevelAsset target)
        {
            SerializedObject = obj;
            Target = target;
            height= Gl.Height(EditorGUIUtility.singleLineHeight * 2);
            properties = new Properties(this.SerializedObject);
             GeneratePhaseList();
             SelectedPhase = properties.EditorSelectedPhase.intValue;
             phaseList.Select(SelectedPhase,false);
          
        }
      
        public  void OnInspectorGUI()
        {
            boldThickLabel = new GUIStyle(EditorStyles.boldLabel);
            boldThickLabel.fontSize = 20;
            SerializedObject.UpdateIfRequiredOrScript();
            
            PrepareSelectedPhase();
            Color old = GUI.color;
            if(properties.Skip.boolValue)
                GUI.color = Color.red;
            Gl.Label(Target.ToString(),boldThickLabel);
            Gl.Label($"{(int)Target.Time/60}m {Mathf.Round(Target.Time%60)}s");
            GUI.color = old;
            DrawMainButtons();
            
            Egl.BeginHorizontal(Gl.MaxWidth(1000),Gl.MinWidth(100));
            
            Egl.BeginVertical(Gl.Width(70));
            DrawControlSelectedPhaseColumn();
            Egl.EndVertical();
            
            
            Egl.BeginVertical();
           
            DrawSelectedPhase();
            Egl.EndVertical();;
            
            Egl.EndHorizontal();;
            
            properties.EditorSelectedPhase.intValue = SelectedPhase;
            
            SerializedObject.ApplyModifiedProperties();
            


        }
        private void GeneratePhaseList()
        {

             phaseList = new ReorderableList(SerializedObject, properties.Phases);
            phaseList.drawElementCallback += (Rect rect,
                int index,
                bool isActive,
                bool isFocused) =>
            {
                string stringName = properties.Phases.GetArrayElementAtIndex(index).FindPropertyRelative("phaseName").stringValue;
                if (string.IsNullOrEmpty((stringName?.Trim())))
                    stringName = index.ToString();
                GUI.Label(rect, stringName);

            };
            phaseList.headerHeight = 0;
            phaseList.onSelectCallback += (ReorderableList list) =>
            {
                int index = phaseList.selectedIndices[0];
                SelectedPhase = index;
            };
            phaseList.onAddCallback += (list =>
            {
                properties.Phases.arraySize++;
                if (properties.Phases.arraySize >= 2)// if there's more than two there could be copy linking incident
                {
                    var type = instance.Phases[properties.Phases.arraySize - 2].SpawnController?.GetType();
                    if (type != null)
                        properties.Phases.GetArrayElementAtIndex(properties.Phases.arraySize - 1).FindPropertyRelative("spawnController")// i can set the value without really allowing it also obj probably wouldn't get deserialized anyway
                            .managedReferenceValue = Activator.CreateInstance(type);
                }
                
            });
        }
       
        private void PrepareSelectedPhase()
        {

            SelectedPhase = properties.EditorSelectedPhase.intValue;
          
        }
        private void DrawMainButtons()
        {

            Egl.BeginHorizontal();
            if (Gl.Button("▶", height, Gl.Width(50)))
            {
                if(requireValidationToStart)
                    DebugLevelRunner.StartByGameAssetWithValidation(Target);
                else
                    DebugLevelRunner.StartByGameAsset(Target);
                
            }
            
            if (Gl.Button("?",height, Gl.Width(50)))
            {
                ValidateResult result = ValidatorSystem.Validate(Target);
                if (result.IsOk)
                    Debug.Log(result);
                else
                    Debug.LogError(result);
            }
            
            Egl.BeginVertical();
            var h = Gl.Height(EditorGUIUtility.singleLineHeight);
            requireValidationToStart = Gl.Toggle(requireValidationToStart, "R", h, Gl.Width(35));
            properties.Skip.boolValue = Gl.Toggle(properties.Skip.boolValue, "->", h, Gl.Width(35));
            Egl.EndVertical();
           

            Egl.EndHorizontal();
        }
        private void DrawSelectedPhase()
        {

            EditorGUILayout.BeginHorizontal(Gl.MinWidth(20),height);
            if(SelectedPhase>=0)
                Gl.Label($"{SelectedPhase}", boldThickLabel);
            
            EditorGUILayout.EndHorizontal();
            if (SelectedPhase == -2)
            {
                DrawAdditionalProperties();
            }
            else if (SelectedPhase != -1 && SelectedPhase< properties.Phases.arraySize)
            {
                PhaseProperties phaseProperties = new PhaseProperties(properties.Phases.GetArrayElementAtIndex(SelectedPhase));
                Egl.PropertyField(phaseProperties.Elements);
                Phase phase = instance.Phases[SelectedPhase];
                ISpawnController controller = phase.SpawnController;
                DrawSpawnControllerField(phaseProperties, phase,controller);
                EditorGUILayout.Space();
                Egl.PropertyField(phaseProperties.PhaseName);
            }
           
            
          
        }
        private void DrawSpawnControllerField(in PhaseProperties phaseProperties,Phase phase, ISpawnController controller)
        {

            Egl.BeginHorizontal();
            {
                Egl.HelpBox("In theory that should not never happened but there's small chance of corruption", MessageType.Info, true);
                if (Gl.Button("Check & Fix"))
                {
                    foreach (var level in GameAsset.Current.Levels)// possible it is even impossible to have such corruption in other levels, tho just to be sure
                    {
                        var result = level.ValidateIndividualityOfController(phase);
                        if (result != null)
                        {
                            phaseProperties.SpawnController.managedReferenceValue = Activator.CreateInstance( phase.SpawnController.GetType());
                            Debug.Log("Link corruption detected and fixed");
                            break;
                        }
                    }
                    Debug.Log("No need to fix anything");
                }
               
            }Egl.EndHorizontal();
          
            Egl.PropertyField(phaseProperties.SpawnController);
           
            if (controller != null)
                Egl.HelpBox(controller?.GetDescription(phase), MessageType.None);
            Egl.Space();
            if (SelectedPhase != 0)
                Egl.PropertyField(phaseProperties.UseAllFromBefore);
        }

        private void DrawControlSelectedPhaseColumn()
        {
            
            phaseList.DoLayoutList();
            if (Gl.Button("Values"))
            {
                SelectedPhase = -2;
            }
        }
        private void DrawAdditionalProperties()
        {
            
          
            Egl.PropertyField(properties.Scene);
            Egl.PropertyField(properties.AdditionalObjectsToSpawn);
            Egl.PropertyField(properties.StartText);
            Egl.PropertyField(properties.PunishForWrong);
            EditorGUI.BeginChangeCheck();
            float before = properties.StartHp.floatValue;
            Egl.PropertyField(properties.StartHp);
            if (EditorGUI.EndChangeCheck())
            {
                ScaleRules(before);
            }
            Egl.PropertyField(properties.AngleError);
            Egl.PropertyField(properties.LevelName);
            DrawRankRules();
            Egl.PropertyField(properties.Cutscene);
            Egl.PropertyField(properties.AfterCutscene);
            Egl.PropertyField(properties.Background);
            Egl.PropertyField(properties.CustomMusic);
          



        }
        private void ScaleRules(float before)
        {
            var minHpCurveProperty = properties.RankRules.FindPropertyRelative("hpMin");
            float startHp = this.properties.StartHp.floatValue;
            var reference = minHpCurveProperty.animationCurveValue;
            var len = reference.length;
            float ratio = startHp / before;
            for (var index = 0; index <len ; index++)
            {
                Keyframe key = minHpCurveProperty.animationCurveValue.keys[index];
                reference.MoveKey(index, new Keyframe(key.time, key.value* ratio));
            }
            minHpCurveProperty.animationCurveValue = reference;//99% don't have to do that
        }
        private void DrawRankRules()
        {

           
            Egl.PropertyField(properties.DisableRanks);
            if (properties.DisableRanks.boolValue)
            {
                Egl.HelpBox($"You will always get an unknown rank",MessageType.Info);
                return;
            }
            Egl.LabelField("Rank Rules",new GUIStyle("label"){fontStyle = FontStyle.Bold});
            
            Eg.indentLevel++;
            properties.UseRankRulesAsset.boolValue= Egl.Toggle(new GUIContent( "use asset"), properties.UseRankRulesAsset.boolValue);

            float[] hValue = null;
            float[] pValue = null;

            if (!properties.UseRankRulesAsset.boolValue)
            {
                var rankProp = properties.RankRules;
                var hpMinProp = rankProp.FindPropertyRelative("hpMin");
                var perfectMinProp = rankProp.FindPropertyRelative("perfectMin");
             
                Egl.PropertyField(hpMinProp);
                Egl.PropertyField(perfectMinProp);
               
                var hpCurve = hpMinProp.animationCurveValue;
                var pCurve =perfectMinProp .animationCurveValue;
                hValue = Enumerable.Range(0,4).Select(i=>hpCurve.Evaluate(i)).ToArray() ;
                pValue =Enumerable.Range(0,4).Select(i=>pCurve.Evaluate(i)).ToArray() ;

            }
            else
            {
                Egl.PropertyField(properties.RankRulesAsset,true);
                    var asset = properties.RankRulesAsset.objectReferenceValue as RankRulesAsset;
                    if (asset != null)
                    {
                        hValue = Enumerable.Range(0,4).Select(i=>asset.Rules.EvaluateHp(i)).ToArray() ;
                        pValue =Enumerable.Range(0,4).Select(i=>asset.Rules.EvaluatePerfect(i)).ToArray() ;
                    }
                
            }
        
           
            if(hValue!=null)
                EditorGUILayout.HelpBox(Enumerable.Range(0, 4).BuildString(skipLast:true,getter: (i, _) =>
                {
                    return $"{(RankLevel)(i+1)}: hp >= {Mathf.Ceil( hValue[i] *properties.StartHp.floatValue *10f)/10} and perfect >=  {Mathf.Ceil( pValue[i]*10f)/10}\n";
                }),MessageType.Info);
            Eg.indentLevel--;
           Egl.Space();
               



        }
    }
}
#endif