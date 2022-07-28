using System;
using System.Collections.Generic;
using Cyberultimate;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LetterBattle
{

    [Serializable]
    public class  UISelectionList
    {
        public Text Text;
        public Button Left, Right;
        private Func<int, string> getTextFunc;
        public event Action<int> OnChanged;

        private static int Wrap(int v, int exclusive)
        {
            if (v < 0) return exclusive + v;
            else return v % exclusive;
        }
        public void Initialize(Func<int, string> f,int len)
        {
           
            getTextFunc = f;
            Left.onClick.AddListener(()=>Index=Wrap(Index-1,len));
            Right.onClick.AddListener(()=>Index=Wrap(Index+1,len));
            
        }
        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;
                _Index = value;
                Text.text = getTextFunc(value);
                OnChanged?.Invoke(value);
            }
        }
        private int _Index;
    }
    public class OptionsManager : MonoBehaviour
    {
        [SerializeField] private UISelectionList resolutionsEntity;
        [SerializeField] private UISelectionList fpsLimitEntity;
        [SerializeField] private Toggle vSyncEntity;
        [SerializeField] private Slider masterVolumeEntity;
        [SerializeField] private Slider musicVolumeEntity;
        [SerializeField] private Slider soundVolumeEntity;
        [SerializeField] private Toggle fullscreenEntity;


        private static int[] FpsOptions = new[] {15,30, 60, 75, 90, 120, 144,165,240,360, 420,-1};
        
        private void LoadSavedValues()
        {
            SettingsData cur = SettingSystem.Current;
            vSyncEntity.isOn = cur.VSync;
            masterVolumeEntity.value = cur.MasterVolume;
            musicVolumeEntity.value = cur.MusicVolume;
            soundVolumeEntity.value = cur.SoundVolume;
            resolutionsEntity.Index =
                Screen.resolutions.Index(resolutionData => resolutionData.width == cur.Resolution.x && resolutionData.height == cur.Resolution.y && resolutionData.refreshRate == cur.Resolution.hz)
                ?? (Screen.resolutions.Length - 1);

            fpsLimitEntity.Index = FpsOptions.Index(item => item == cur.MaxFps) ?? FpsOptions[^1];
            fullscreenEntity.isOn = cur.FullScreen;

        }

        private void AddListenerWithApply(UnityEvent<float> ev, Action<float> act)
        {
            ev.AddListener((a) =>
            {
                act(a);
                SettingSystem.Current.Apply();
            });
        }
        private void Awake()
        {
            resolutionsEntity.Initialize((i) => $"{Screen.resolutions[i].width}x{Screen.resolutions[i].height}, {Screen.resolutions[i].refreshRate}Hz", Screen.resolutions.Length);
            fpsLimitEntity.Initialize((i) => (FpsOptions[i] == -1) ? "∞" : FpsOptions[i].ToString(),FpsOptions.Length);
          
            LoadSavedValues();
            
            vSyncEntity.onValueChanged.AddListener(value => SettingSystem.Current.VSync = value);
            AddListenerWithApply(masterVolumeEntity.onValueChanged,value=>SettingSystem.Current.MasterVolume=value);
            AddListenerWithApply(musicVolumeEntity.onValueChanged,value=>SettingSystem.Current.MusicVolume=value);
            AddListenerWithApply(soundVolumeEntity.onValueChanged,value=>SettingSystem.Current.SoundVolume=value);
            
            fullscreenEntity.onValueChanged.AddListener(value=>SettingSystem.Current.FullScreen=value);
            resolutionsEntity.OnChanged += index =>
                SettingSystem.Current.Resolution = (Screen.resolutions[index].width, Screen.resolutions[index].height, Screen.resolutions[index].refreshRate);
            fpsLimitEntity.OnChanged += index => SettingSystem.Current.MaxFps = FpsOptions[index];
         
        }

        public void ApplyChanges()
        {
            SettingSystem.Current.Apply();
            SettingSystem.Save();
            ;
        }
        
    }
}