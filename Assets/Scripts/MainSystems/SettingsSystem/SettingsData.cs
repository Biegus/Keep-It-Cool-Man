using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Audio;

namespace LetterBattle
{
    [DataContract]
    public class SettingsData
    {
        private static readonly Lazy<AudioMixer> mixer = new(() => Resources.LoadAll<AudioMixer>("").First());
       
        [DataMember(IsRequired = false)] public (int x, int y, int hz) Resolution { get; set; } = (1920, 1080, 75);
        [DataMember(IsRequired = false)] public bool FullScreen { get; set; } = true;
        [DataMember(IsRequired = false)] public bool VSync { get; set; } = true;
        [DataMember(IsRequired = false)] public float MasterVolume { get; set; } = 1;
        [DataMember(IsRequired = false)] public int MaxFps { get; set; } = -1;
        [DataMember(IsRequired = false)] public float MusicVolume { get; set; } = 1;
        [DataMember(IsRequired = false)] public float SoundVolume { get; set; } = 1;


        private static  float ConvertToDb(float v)
        {
            if (v == 0) return -80;
            else return Mathf.Log(v) * 20;
        }
        private static void SetVolume(string name, float value)
        {
            mixer.Value.SetFloat(name, ConvertToDb(value));
        }
        public void Apply()
        {
            Screen.SetResolution(Resolution.x, Resolution.y, FullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, Resolution.hz);
            QualitySettings.vSyncCount = VSync.ToInt();
           
            Application.targetFrameRate = MaxFps;
            SetVolume("masterVolume",MasterVolume);
            SetVolume("musicVolume",MusicVolume);
            SetVolume("sfxVolume",SoundVolume);
            
        }
    }
}