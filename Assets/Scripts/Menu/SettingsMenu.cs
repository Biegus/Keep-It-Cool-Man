using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
    public class SettingsMenu : MonoBehaviour
    {
        public enum Setting
        {
            Gameplay,
            Video,
            Audio,
        }

        [SerializeField]
        private Button backBtn;

        [SerializeField]
        private GameObject[] settingPanels;

        [SerializeField]
        private GameObject categorySelectionPanel;

        [SerializeField]
        private SettingsAnimationHandler animationHandler;

        private void Start()
        {
            backBtn.onClick.AddListener(animationHandler.OnBackBtn);
        }

        private void OnAnySettingPress(int lastSetting)
        {
            categorySelectionPanel.SetActive(false);
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(() => GoCategorySelection(lastSetting));
        }

        private void GoCategorySelection(int lastSetting)
        {
            categorySelectionPanel.SetActive(true);
            settingPanels[lastSetting].SetActive(false);
            backBtn.onClick.RemoveAllListeners();
            backBtn.onClick.AddListener(animationHandler.OnBackBtn);
        }

        public void OnSettingPress(int setting)
        {
            foreach (var panel in settingPanels)
            {
                panel.SetActive(panel == settingPanels[setting]);
            }
            OnAnySettingPress(setting);
        }
    }
}

