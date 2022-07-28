#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace LetterBattle.Editor
{
    public class GlobalLevelEditorWindow : EditorWindow
    {
        [MenuItem("Kicm/Global Level editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<GlobalLevelEditorWindow>();
            window.titleContent = new GUIContent("Global Level Editor");
            window.Show();
        }
        private LevelAsset asset;
        private LevelAssetSoftDrawer drawer;
        private ReorderableList[] reorderables;
        bool brokenMode = false;
        private string nwFileName;
        private Vector2 selectScroll;
        private Vector2 elementScroll;

        private static readonly string ASSET_KEY = $"{nameof(GlobalLevelEditorWindow)}/openedLv";
        private void OnEnable()
        {
            if (EditorPrefs.HasKey(ASSET_KEY))
            {
                asset = GameAsset.Current.Levels[EditorPrefs.GetInt(ASSET_KEY)];
            }
               
        }
        private void OnDisable()
        {
            var lvId = GameAsset.Current.GetIndexOflevel(asset);
            if(lvId!=null)
            EditorPrefs.SetInt(ASSET_KEY,lvId.Value);
            else
                EditorPrefs.DeleteKey(ASSET_KEY);
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();
            if (reorderables == null && brokenMode== false)
            {
                ForceRebuildReorderables();
            }
            asset = (LevelAsset) EditorGUILayout.ObjectField(asset,typeof(LevelAsset),true);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(150));

            if (!brokenMode)
            {
                DrawLevelSelect();
            }
            else
            {
                GUILayout.Label("Null levels detected");
                if (GUILayout.Button("Fix"))
                {
                    for (int i = 0; i < GameAsset.Current.Chapters.Count; i++)
                    {
                        var eRef = GameAsset.Current.Chapters[i].GetEditorRef();
                        eRef.RemoveAll(item => item == null);
                    }
                    brokenMode = false;

                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (asset != null)
            {
                if (drawer == null || drawer.Target != asset)
                {
                  
                    drawer = new LevelAssetSoftDrawer(new SerializedObject(asset), asset);
                }
                elementScroll= EditorGUILayout.BeginScrollView(elementScroll);
                drawer.OnInspectorGUI();
                
                if (drawer.SelectedPhase == -2)
                {
                    EditorGUILayout.Space(20);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Change file name");
                    nwFileName= EditorGUILayout.TextField(nwFileName);
                    if(GUILayout.Button("Go"))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(asset);
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                        if (nwFileName != assetPath)
                        {
                            AssetDatabase.MoveAsset(assetPath,$"{assetPath.Substring(0, assetPath.IndexOf(fileName))}{nwFileName}.asset");
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
              
                
              

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawLevelSelect()
        {
          
            if (GUILayout.Button("Force refresh",GUILayout.Width(100)))
            {
                ForceRebuildReorderables();
            }
            selectScroll = EditorGUILayout.BeginScrollView(selectScroll);
            foreach (var reorderableList in reorderables)
            {
                reorderableList.DoLayoutList();
            }
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Open Game asset"))
                Selection.activeObject = GameAsset.Current;
        }
        private void ForceRebuildReorderables()
        {
            reorderables = new ReorderableList[GameAsset.Current.Chapters.Count];
            for (int i = 0; i < reorderables.Length; i++)
            {
                int varI = i;
                ChapterAsset chapter = GameAsset.Current.Chapters[varI];
                reorderables[i] = new ReorderableList(GameAsset.Current.Chapters[i].Levels.Select((item, i) =>
                    {
                        if (item == null)
                        {
                            brokenMode = true;
                            return string.Empty;
                        }
                        return $"{GameAsset.Current.FromCertainToRaw(varI, i)}:{item.ToString()}";
                    }).ToArray(), typeof(string),
                    draggable: false, displayHeader: true, displayAddButton: true, displayRemoveButton: false);
               
                if (brokenMode)
                {
                    reorderables = null;
                    break;;
                }
                reorderables[i].onSelectCallback += list =>
                {
                    int selected = reorderables[varI].selectedIndices[0];
                    foreach (var reorderable in reorderables)
                    {
                        reorderable.Select(-1);
                    }
                    reorderables[varI].Select(selected);
                    asset = GameAsset.Current.Chapters[varI].Levels[reorderables[varI].selectedIndices[0]];
                   

                };
                reorderables[i].footerHeight /= 1.2f;
                reorderables[i].drawHeaderCallback += (rect =>
                {
                    EditorGUI.LabelField(rect, $"Chapter {varI + 1}");
                    
                });
                
                reorderables[i].onAddCallback += list =>
                {
                    LevelAsset asset = ScriptableObject.CreateInstance<LevelAsset>();

                    string fileName = $"Lv_{varI+1}_{chapter.Levels.Count + 1}.asset";
                    if (string.IsNullOrEmpty(chapter.DefaultLevelPath))
                    {
                        Debug.LogWarning("Chapter doesn't define default level path");
                        AssetDatabase.CreateAsset(asset, $"Assets/Resources/Content/Levels/{fileName}");
                    }
                    
                    else
                        AssetDatabase.CreateAsset(asset, $"{chapter.DefaultLevelPath}/{fileName}");
                    GameAsset.Current.Chapters[varI].GetEditorRef().Add(asset);
                    ForceRebuildReorderables();

                };
            }
        }
    }
}

#endif