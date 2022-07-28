using System;
using System.Linq;
using Cyberultimate;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace LetterBattle
{
    public interface ICutsceneEffect
    {
         void Play(CutsceneManager controller);
       
    }
    public class ShakeCameraCutsceneEffect : ICutsceneEffect
    {
        [SerializeField] private bool finite = true;
        
        public void Play(CutsceneManager controller)
        {
            controller.PlayStatic(CameraHelper.Current.ShakeScreen(finite?40:int.MaxValue));
        }
        

    }
    [Serializable]
    public class PlainDialogueElement
    {
      
        [NaughtyAttributes.ResizableTextArea]
        public string Text;
        [UseSharedTemplate("Dialogue/Who")]
        public string Who;
        [NaughtyAttributes.ShowAssetPreview()]
        [UseSharedTemplate("Dialogue/Sprites")]
        public Sprite Sprite;

        public AudioClip Clip;
        

    }
    [Serializable]
    public class SpecialDialogueElement
    {
        
        [SerializeField]
        public float Time = 2;
        [SerializeField]
        public bool HideTextBox = false;
        [SerializeField]
        [TextArea]
        public string ShowText;
        [SerializeField]
        public string[] EventCalled = new string[0];

        [SerializeField] public Color Color=Color.black;

        [FormerlySerializedAs("changeback")] public bool changeBack;
        [AllowNesting]
        [NaughtyAttributes.ShowIf(nameof(changeBack))]
        public int newBackValue;
        public bool dontFadeOut = false;
        public bool dontFadeIn = false;
    
    }
    public enum CutsceneElementType
    {
        Plain,
        Special,
    }
    [Serializable]
    public class CutsceneElement
    {

        [SerializeReference]
        [SerializeReferenceButton]
        private ICutsceneEffect startEffect;
        [SerializeField]
        private CutsceneElementType type;
        [AllowNesting]
        [NaughtyAttributes.ShowIf (nameof(type),CutsceneElementType.Plain)]
        [SerializeField]
        private PlainDialogueElement PlainContent;
        [AllowNesting]
        [NaughtyAttributes.ShowIf (nameof(type),CutsceneElementType.Special)]
        [SerializeField]
        private SpecialDialogueElement SpecialContent;

        [SerializeField] private bool interrupt = false;
      

        public ICutsceneEffect StartEffect => startEffect;
      
        public bool Interrupt
        {
            get => interrupt;
            set => interrupt = value;
        }
        public CutsceneElementType Type
        {
            get => type;
            set => type=value;
        }

       

        public PlainDialogueElement GetPlain()
        {
            if (type == CutsceneElementType.Plain) return PlainContent;
            else return null;
        }
        public SpecialDialogueElement GetSpecial()
        {
            if (type == CutsceneElementType.Special) return SpecialContent;
            else return null;
        }


    }
    
   
}