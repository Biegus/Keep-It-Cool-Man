using System;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class MoveToCenterOnLevelEnded : MonoBehaviour
    {
        [SerializeField][MinValue(0f)] private float transitionTime = 2f;
        
        [SerializeField] private Transform center;

        [SerializeField] private bool itself = true;
        [SerializeField] [NaughtyAttributes.HideIf(nameof(itself))] private Transform obj;
        
        private Transform ObjToMove
        {
            get
            {
                if (itself) return this.transform;
                else
                {
                  
                    if (obj == null)
                    {
                       
                        Debug.LogError("Mode is not set to itself and no object was provided. Itself was returned");
                        return this.transform;

                    }
                    return obj;

                }
            }
        }
        private void OnEnable()
        {
            LEvents.Base.OnLevelWon.Register(OnLevelWon);
        }
        private void OnDisable()
        {
            LEvents.Base.OnLevelWon.Unregister(OnLevelWon);
        }
        private void OnLevelWon(object sender, EventArgs e)
        {
            if (LevelManager.Current.Targets.Count == 0)
            {
                throw new InvalidOperationException("There's no target regirstered");
            }
            this.ObjToMove.DOMove(center.position, transitionTime).SetLink(this.ObjToMove.gameObject)
                .SetUpdate(true);
        }
    }
}