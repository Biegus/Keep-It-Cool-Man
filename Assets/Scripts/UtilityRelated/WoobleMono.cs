using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle
{
    [RequireComponent(typeof(LineRenderer))]
    public class WoobleMono : MonoBehaviour
    {
        private LineRenderer lineRender;
        private void Awake()
        {
            lineRender = this.GetComponent<LineRenderer>();
        }
        private void Start()
        {
            for (int i = 1; i < lineRender.positionCount; i += 2)
            {
                int varLocal = i;
                Vector2 local = lineRender.GetPosition(varLocal);
                DOTween.To(() => ( Vector2) lineRender.GetPosition(varLocal),(v)=> lineRender.SetPosition(varLocal,v),local- new Vector2(0,0.2f),0.4f)
                    .SetLoops(-1,LoopType.Yoyo);
            }
        }
    }
}