using System;
using UnityEngine;
namespace LetterBattle
{
    public class JustSayExtension : MonoBehaviour
    {
        [SerializeField] private string text="hi";
        [SerializeField] private int times = 4;
        private void Start()
        {
            for(int i=0;i<times;i++)
            InfoBoxSpawner.Current.Spawn(null,text,GameAsset.Current.Pallete.GetColor(ColorType.Danger),4);
        }
    }
}