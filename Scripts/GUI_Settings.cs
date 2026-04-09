using System;
using System.Collections;
using System.Collections.Generic;
using NamPhuThuy.DataManage;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.UGUIImplement
{
    
    public class GUI_Settings : GUIBase
    {
        #region Private Serializable Fields

        [Header("Buttons")]
        [SerializeField] private ButtonClicky closeButton;
        [SerializeField] private ButtonClicky languageButton;
        [SerializeField] private ButtonClicky supportButton;
        [SerializeField] private ButtonClicky privacyPolicyButton;
        [SerializeField] private ButtonClicky termsOfServiceButton;
        [SerializeField] private ButtonClicky rateButton;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI versionText;
        
        [Header("Audio Settings")]
        [SerializeField] private ButtonClicky musicButton;
        [SerializeField] private ButtonClicky sfxButton;
        [SerializeField] private ButtonClicky vibrationButton;
        [SerializeField] private ButtonClicky notificationButton;
        
        #endregion

        #region Private Fields

        

        #endregion

        #region MonoBehaviour Callbacks
        
        private void Start()
        {
            versionText.text = $"Version {Application.version}";
            SetStateForButton();
        }

        

        void OnEnable()
        {
            closeButton.onClick.AddListener(OnClickClose);
            rateButton.onClick.AddListener(OnClickRate);
            languageButton.onClick.AddListener(OnClickLanguage);
            supportButton.onClick.AddListener(OnClickSupport);
            privacyPolicyButton.onClick.AddListener(OnClickPrivacyPolicy);
            termsOfServiceButton.onClick.AddListener(OnClickTermsOfService);
            
            // AUDIO SETTINGS
            musicButton.onClick.AddListener(OnClickMusic);
            sfxButton.onClick.AddListener(OnClickSound);
            vibrationButton.onClick.AddListener(OnClickVibration);
            notificationButton.onClick.AddListener(OnClickNotification);
        }

        
     
        #endregion

        #region Private Methods
        
        private void SetStateForButton()
        {
            bool tmp = true/*AudioManager.Ins.IS_MUSIC_ON*/;
            ButtonSwitchFade musicSwitch = musicButton.GetComponent<ButtonSwitchFade>();
            if (musicSwitch != null)
            {
                if (tmp) musicSwitch.SetState(ButtonSwitchFade.State.ON);
                else musicSwitch.SetState(ButtonSwitchFade.State.OFF);
            }
            
            tmp = true/*AudioManager.Ins.IS_SOUND_ON*/;
            ButtonSwitchFade sfxSwitch = sfxButton.GetComponent<ButtonSwitchFade>();
            if (sfxSwitch != null)
            {
                if (tmp) sfxSwitch.SetState(ButtonSwitchFade.State.ON);
                else sfxSwitch.SetState(ButtonSwitchFade.State.OFF);
            }
            
            tmp = DataManager.Ins.PSettingsData.vibrationEnabled;
            ButtonSwitchFade vibrateSwitch = vibrationButton.GetComponent<ButtonSwitchFade>();
            if (vibrateSwitch != null)
            {
                if (tmp) vibrateSwitch.SetState(ButtonSwitchFade.State.ON);
                else vibrateSwitch.SetState(ButtonSwitchFade.State.OFF);
            }
            
            tmp = DataManager.Ins.PSettingsData.notifyEnabled;
            ButtonSwitchFade notifySwitch = notificationButton.GetComponent<ButtonSwitchFade>();
            if (notifySwitch != null)
            {
                if (tmp) notifySwitch.SetState(ButtonSwitchFade.State.ON);
                else notifySwitch.SetState(ButtonSwitchFade.State.OFF);
            }
        }
        
        #endregion

        #region Public Methods
        #endregion

        #region Button Events

        private void OnClickClose()
        {
            Hide();
        }

        private void OnClickRate()
        {
            // GUIManager.Ins.ShowGUI(GUIManager.Ins.GUIRating);
        }
        
        private void OnClickLanguage()
        {
            UGUIManager.Ins.ShowGUI(UGUIManager.Ins.GUILanguage);
        }

        private void OnClickSupport()
        {
            throw new NotImplementedException();
        }

        private void OnClickPrivacyPolicy()
        {
            throw new NotImplementedException();
        }

        private void OnClickTermsOfService()
        {
            throw new NotImplementedException();
        }

        private void OnClickMusic()
        {
            bool tmp = DataManager.Ins.PSettingsData.musicEnabled;
            tmp = !tmp;
            DataManager.Ins.PSettingsData.musicEnabled = tmp;
#if USE_AUDIO
            AudioManager.Ins.IS_MUSIC_ON = tmp;
#endif
            DataManager.Ins.MarkDirty();
            
            ButtonSwitchFade musicSwitch = musicButton.GetComponent<ButtonSwitchFade>();
            if (musicSwitch != null)
            {
                if (tmp)
                {
                    musicSwitch.SetState(ButtonSwitchFade.State.ON);
                }
                else
                {
                    musicSwitch.SetState(ButtonSwitchFade.State.OFF);
                }
                
            }
        }

        private void OnClickSound()
        {
            bool tmp = DataManager.Ins.PSettingsData.sfxEnabled;
            tmp = !tmp;
            DataManager.Ins.PSettingsData.sfxEnabled = tmp;
#if USE_AUDIO
            AudioManager.Ins.IS_SOUND_ON = tmp;
#endif
            DataManager.Ins.MarkDirty();
            
            ButtonSwitchFade sfxSwitch = sfxButton.GetComponent<ButtonSwitchFade>();
            if (sfxSwitch != null)
            {
                if (tmp)
                {
                    sfxSwitch.SetState(ButtonSwitchFade.State.ON);
                }
                else
                {
                    sfxSwitch.SetState(ButtonSwitchFade.State.OFF);
                }
                
            }
        }

        private void OnClickVibration()
        {
            bool tmp = DataManager.Ins.PSettingsData.vibrationEnabled;
            tmp = !tmp;
            DataManager.Ins.PSettingsData.vibrationEnabled = tmp;
            DataManager.Ins.MarkDirty();
            
            ButtonSwitchFade vibrateSwitch = vibrationButton.GetComponent<ButtonSwitchFade>();
            if (vibrateSwitch != null)
            {
                if (tmp)
                {
                    vibrateSwitch.SetState(ButtonSwitchFade.State.ON);
                }
                else
                {
                    vibrateSwitch.SetState(ButtonSwitchFade.State.OFF);
                }
                
            }
        }

        private void OnClickNotification()
        {
            bool tmp = DataManager.Ins.PSettingsData.notifyEnabled;
            tmp = !tmp;
            DataManager.Ins.PSettingsData.notifyEnabled = tmp;
            DataManager.Ins.MarkDirty();
            
            ButtonSwitchFade notifySwitch = notificationButton.GetComponent<ButtonSwitchFade>();
            if (notifySwitch != null)
            {
                if (tmp)
                {
                    notifySwitch.SetState(ButtonSwitchFade.State.ON);
                }
                else
                {
                    notifySwitch.SetState(ButtonSwitchFade.State.OFF);
                }
                
            }
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnClickClose);
            languageButton.onClick.RemoveAllListeners();
            supportButton.onClick.RemoveAllListeners();
            privacyPolicyButton.onClick.RemoveAllListeners();
            termsOfServiceButton.onClick.RemoveAllListeners();
            // AUDIO SETTINGS
            musicButton.onClick.RemoveAllListeners();
            sfxButton.onClick.RemoveAllListeners();
            vibrationButton.onClick.RemoveAllListeners();
            notificationButton.onClick.RemoveAllListeners();
        }

        #endregion
    }
}