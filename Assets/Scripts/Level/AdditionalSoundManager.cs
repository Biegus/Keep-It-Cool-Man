using System;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class AdditionalSoundManager : MonoBehaviour
    {

        [SerializeField]
        [Required()]
        private AudioSource source = null;
        private void Awake()
        {
            LEvents.Base.OnLevelWon.Raw += OnLevelWon;
          
        }
        private void OnLevelWon(object sender, EventArgs e)
        {
            source.PlayOneShot(GameAsset.Current.LevelFinishedAudio);
        }
        private void OnDestroy()
        {
            LEvents.Base.OnLevelWon.Raw -= OnLevelWon;
            
        }
    }
}