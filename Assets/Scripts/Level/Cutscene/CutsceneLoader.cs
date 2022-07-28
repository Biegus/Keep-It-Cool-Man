using System;
using UnityEngine;
namespace LetterBattle
{
    public sealed class CutsceneLoader : MonoBehaviour
    {
        [SerializeField] private CutsceneManager manager;
        private void Start()
        {
            if (GameManager.CurrentLevel!=null)
            {
                manager.Dispose();
                manager.Load(MainMenuHandler.CutsceneAsset);
                manager.OnCutsceneEnded += OnCutsceneEnded;
            }
        }
        private void OnDestroy()
        {
            if(manager!=null)
                manager.OnCutsceneEnded -= OnCutsceneEnded;
        }
        private void OnCutsceneEnded(object sender, CutsceneAsset e)
        {
           GameManager.CallCutsceneEnded();
        }
    }
}